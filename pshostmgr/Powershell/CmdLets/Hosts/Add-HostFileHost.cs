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
using System.Linq;
using System.Management.Automation;
using System.Collections.Generic;

namespace ManageHosts.Powershell.CmdLets
{
	using Services;
	using Utility;

	/// <summary>
	/// Cmdlet object for adding a new host entry.
	/// </summary>
	[Cmdlet(VerbsCommon.Add, "HfHost")]
	public sealed class AddHostFileHost : ServiceSupportedCmdLet
	{
		/// <summary>
		/// A new entry must be given a name. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(
			Mandatory	= true, 
			HelpMessage = "Enter a host name ")]
		[Alias("Host")]
		public string Hostname { get; set; }

		/// <summary>
		/// A new entry must be given an address. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(
			Mandatory	= true, 
			HelpMessage = "Enter an IP address ")]
		[Alias("IP")]
		public string Address { get; set; }

		/// <summary>
		/// Execution the command invocation by attempting
		/// to add a new host entry.
		/// </summary>
		protected override void ProcessRecord()
		{
			var service = HostFileService;
			var entries = service.GetEntries();

			// can't add a duplicate. Throw error...
			if (entries.Any(x => x.Hostname.AreHostFileStringEqual(Hostname)))
				throw new DuplicateHostException(Hostname);

			var entry = new HostFileEntry();
			entry.Hostname = Hostname;
			entry.Address = Address;
			entry.SetTypeFromAddress();

			var entryList = new List<HostFileEntry>();
			entryList.AddRange(entries);
			entryList.Add(entry);

			service.WriteEntries(entryList);
			WriteObject(entry);

			Log.WriteLog($"Added host to host file: {entry.ToString()}");

			// END FUNCTION
		}

		// END CLASS (AddHostFileHost)
	}

	// END NAMESPACE
}
