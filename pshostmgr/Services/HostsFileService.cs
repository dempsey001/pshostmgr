/*
 * MIT License
 * 
 * Copyright (c) 2016 Joseph Dempsey
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AddrFamily = System.Net.Sockets.AddressFamily;

namespace ManageHosts.Services
{
    using Utility;

    /// <summary>
    /// Static class to hold some basic constants related
    /// to the reading/writing of host files.
    /// </summary>
    internal static class HostFileConstants
    {
        /// <summary>
        /// Generic Microsoft header value retrieved from a hosts file
        /// on Windows 10. This is added to the top of all hosts file
        /// for consistency.
        /// </summary>
        public static readonly string MicrosoftHostFileHeader = new StringBuilder().Append(
            "# Copyright (c) 1993-2009 Microsoft Corp.").Append(Environment.NewLine).Append(
            "#").Append(Environment.NewLine).Append(
            "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.").Append(Environment.NewLine).Append(
            "#").Append(Environment.NewLine).Append(
            "# This file contains the mappings of IP addresses to host names. Each").Append(Environment.NewLine).Append(
            "# entry should be kept on an individual line. The IP address should").Append(Environment.NewLine).Append(
            "# be placed in the first column followed by the corresponding host name.").Append(Environment.NewLine).Append(
            "# The IP address and the host name should be separated by at least one").Append(Environment.NewLine).Append(
            "# space.").Append(Environment.NewLine).Append(
            "#").Append(Environment.NewLine).Append(
            "# Additionally, comments (such as these) may be inserted on individual").Append(Environment.NewLine).Append(
            "# lines or following the machine name denoted by a '#' symbol.").Append(Environment.NewLine).Append(
            "#").Append(Environment.NewLine).Append(
            "# For example:").Append(Environment.NewLine).Append(
            "#").Append(Environment.NewLine).Append(
            "#      102.54.94.97     rhino.acme.com          # source server").Append(Environment.NewLine).Append(
            "#       38.25.63.10     x.acme.com              # x client host").Append(Environment.NewLine).Append(
            "").Append(Environment.NewLine).Append(
            "# localhost name resolution is handled within DNS itself.").Append(Environment.NewLine).Append(
            "#	    127.0.0.1       localhost").Append(Environment.NewLine).Append(
            "#	    ::1             localhost").Append(Environment.NewLine).ToString();


        /// <summary>
        /// A small disclaimer that is written into the host file
        /// noting it was modified but this utility.
        /// </summary>
        public static string PsHostsDisclaimer =>
            $"# MODIFIED BY HOST MANAGER: {DateTime.Now}";

        // END CLASS (HostFileConstants)
    }

    /// <summary>
    /// Enum to represent the type of data on a given line.
    /// </summary>
    internal enum HostFileLineEntryType
    {
        /// <summary>
        /// Value not yet acertained.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Host file line contains data for a mapping
        /// of a host name to IPv4 address.
        /// </summary>
        HostForIPv4,

        /// <summary>
        /// Host file line contains data for a mapping
        /// of a host name to IPv6 address.
        /// </summary>
        HostForIPv6,

        /// <summary>
        /// The line contains only comment data.
        /// </summary>
        Comment,

        /// <summary>
        /// Special comment line used to provide
        /// application feedback/control strings.
        /// </summary>
        Control,

        /// <summary>
        /// Blank line.
        /// </summary>
        Blank
    }

	/// <summary>
	/// Represents a single line in in the 
	/// host file after its been parsed.
	/// </summary>
	internal sealed class HostFileEntry : IComparable<HostFileEntry>
    {
		/// <summary>
		/// Associated hostname with the entry.
		/// </summary>
		public string Hostname { get; set; }

        /// <summary>
        /// Associated IP address with the entry.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Denotes address type.
        /// </summary>
        public HostFileLineEntryType Type { get; set; }

		/// <summary>
		/// Dump some useful information for this 
		/// object type.
		/// </summary>
		/// <returns>String with address/hostname data.</returns>
	    public override string ToString()
		{
			return $"{Hostname} => {Address}";
		}

		/// <summary>
		/// Attempts to set the address line type based
		/// on the address contained within.
		/// </summary>
		/// <returns></returns>
		public HostFileLineEntryType SetTypeFromAddress()
		{
			IPAddress ipAddr;
			if (IPAddress.TryParse(Address, out ipAddr))
			{
				Type = (ipAddr.AddressFamily == AddrFamily.InterNetwork)
					? HostFileLineEntryType.HostForIPv4
					: (ipAddr.AddressFamily == AddrFamily.InterNetworkV6)
					? HostFileLineEntryType.HostForIPv6
					: HostFileLineEntryType.Unknown;
			}

			return Type;
		}

		/// <summary>
		/// Returns this host entry data as a line item formatted
		/// for the host file.
		/// </summary>
		/// <param name="addrColLen">Total length of the address column. If zero, normal
		/// length is used.</param>
		/// <returns>The formatted string</returns>
		public string AsHostFileEntryText(int addrColLen = 0)
        {
			var addr = addrColLen != 0 ? Address.PadRight(addrColLen, ' ') : Address;
            return $"{addr}\t{Hostname}";
        }

		/// <summary>
		/// Compares this instance to another. 
		/// </summary>
		/// <param name="other">Instance to compare to.</param>
		/// <returns>-1 if less than, 0 if equal and 1 if greater than 
		/// comparison target.</returns>
		public int CompareTo(HostFileEntry other)
		{
			if (null == other) 
				return 1;
			if(object.ReferenceEquals(this, other))
				return 0;

			var retValue = string.Compare(
				Hostname, 
				other.Hostname, 
				StringExtensions.HostFileStringComparison);
			return retValue != 0 ? retValue 
				: string.Compare(Address, other.Address, StringComparison.Ordinal);
		}

		// END CLASS (HostFileEntry)
	}

    /// <summary>
    /// Parser service for retrieving data from a host file.
    /// </summary>
    internal interface IHostFileDataService
    {
        /// <summary>
        /// Provides an override for a default host file location.
        /// </summary>
        string HostFilePath { get; set; }

        /// <summary>
		/// Retrieves a full list of all entries that are present
		/// in the system's host file.
		/// </summary>
		/// <returns>List of entries</returns>
        IEnumerable<HostFileEntry> GetEntries();

        /// <summary>
		/// Provides functionality to write all host file
		/// entries provided out to the host file.
		/// </summary>
		/// <returns>List of entries</returns>
        void WriteEntries(IEnumerable<HostFileEntry> entries);

        // END INTERFACE
    }


    /// <summary>
    /// Class to aide in operations directly against the host file.
    /// </summary>
    internal sealed class HostsFileService : IHostFileDataService
    {
        // path under system directory to the hosts file.
        private const string DriveSubPath = @"drivers\etc\hosts";
		
        // determines if there is any match at the start of the
        // line without a space or # (comment char)
        private static string EmptyCommentLineRegex = @"[\s]*#{1,}";

		/// <summary>
		/// Quick accessor to retrieve the fixed path to the hosts file.
		/// </summary>
		public static string HostFileSystemPath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.System), DriveSubPath);

        // private field to hold host file path.
        private string _hostFilePath;
        #region IHostFileDataService

        /// <summary>
        /// Provides an override for a default host file location.
        /// </summary>
        public string HostFilePath
        {
            get { return string.IsNullOrEmpty(_hostFilePath) ? HostFileSystemPath : _hostFilePath; }
            set { _hostFilePath = value; }

            // END ACCESSOR (HostFilePath)
        }

        /// <summary>
        /// Retrieves a full list of all entries that are present
        /// in the system's host file.
        /// </summary>
        /// <returns>List of entries</returns>
        public IEnumerable<HostFileEntry> GetEntries()
		{
            // this is ok. StreamReader will dispose of the underlying
            // stream when it is disposed.
            using (var reader = new StreamReader(
                File.Open(HostFilePath, FileMode.Open, FileAccess.Read)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    HostFileLineEntryType entryType;
                    var entry = ParseLine(line, out entryType);
                    if (null == entry)
                        continue;

                    entry.Type = entryType;
                    yield return entry;
                }
            }

		    // END FUNCTION
		}

        /// <summary>
        /// Constructs the full text for a new host file
        /// based off of the provided entries.
        /// </summary>
        /// <returns></returns>
        private string BuildHostFileText(IEnumerable<HostFileEntry> entries)
        {
            var hostFileTextBuilder = new StringBuilder();

			var maxLenAddress = entries.Max(x => x.Address.Length);
			maxLenAddress += 5; // add extra buffer.

			// Add Header
			hostFileTextBuilder.Append(
                HostFileConstants.MicrosoftHostFileHeader);
            
            // add seperator newline for readability.
            hostFileTextBuilder.Append(Environment.NewLine);
			hostFileTextBuilder.Append(Environment.NewLine);

			// tack on disclaimer so we know this file was
			// a generated version.
			hostFileTextBuilder.Append(HostFileConstants.PsHostsDisclaimer);
            hostFileTextBuilder.Append(Environment.NewLine);
			hostFileTextBuilder.Append(Environment.NewLine);

			entries?.ToList().ForEach(en =>
			{
				hostFileTextBuilder.Append(en.AsHostFileEntryText(maxLenAddress));
				hostFileTextBuilder.Append(Environment.NewLine);
			});

            // add trailing newline for readability.
            hostFileTextBuilder.Append(Environment.NewLine);

            return hostFileTextBuilder.ToString();
        }

        /// <summary>
        /// Provides functionality to write all host file
        /// entries provided out to the host file.
        /// </summary>
        /// <returns>List of entries</returns>
        public void WriteEntries(IEnumerable<HostFileEntry> entries)
        {
			// we will write to a temp file and then push-to-overwrite
			// the /etc/hosts file so that we can avoid file lock conflicts
			// and be sure the file contains only the data we want in it.
			var tempFilePath = Path.GetTempFileName();

			// this is ok. StreamWriter will dispose of the underlying
			// stream when it is disposed.
			using (var writer = new StreamWriter(
				File.Open(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
			{
				writer.Write(BuildHostFileText(entries));
				writer.Close();
			}

			File.Copy(tempFilePath, HostFilePath, true);


            // END FUNCTION
        }
        #endregion

        /// <summary>
        /// Takes a single line from the host file and parses
        /// it. A host entry is returned for valid lines.
        /// </summary>
        /// <param name="line">line to parse</param>
        /// <returns>A host entry</returns>
        private static HostFileEntry ParseLine(string line, out HostFileLineEntryType type)
        {
            type = HostFileLineEntryType.Blank;
            if (String.IsNullOrWhiteSpace(line))
                return null;

            var empty = new Regex(EmptyCommentLineRegex);
            if (empty.IsMatch(line) || line.Trim().StartsWith("#"))
            {
                type = HostFileLineEntryType.Comment;
                return null;
            }

			var items = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			if (items.Length < 2)
			{
				type = HostFileLineEntryType.Unknown;
				return null;
			}

			var entry = new HostFileEntry()
			{
				Address = items[0].Trim(),
				Hostname = items[1].Trim()
			};

			type = entry.SetTypeFromAddress();

			return entry;

	        // END FUNCTION
        }
    }

	// END NAMESPACE
}

