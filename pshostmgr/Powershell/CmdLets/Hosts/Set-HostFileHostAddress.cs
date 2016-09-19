/*
 * MIT License
 * 
 * Copyright (c) September 2016 Joseph Dempsey
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
using System.Collections.Generic;
using System.Management.Automation;

namespace ManageHosts.Powershell.CmdLets
{
	using Services;
	using Utility;

	/// <summary>
	/// Allows the destination IP address of a given host
	/// to be retargetted to a new one.
	/// </summary>
	[Cmdlet(VerbsCommon.Set, "HfHostAddress")]
	public sealed class SetHostFileHostAddress : ServiceSupportedCmdLet
	{
		/// <summary>
		/// A new entry must be given a name. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(
			Mandatory	= true,
			HelpMessage = "Enter a host ",
			Position	= 0)]
		[Alias("Host")]
		public string Hostname { get; set; }

		/// <summary>
		/// A new entry must be given a name. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(
			Mandatory	= true, 
			HelpMessage	= "Enter an address ")]
		[Alias("IP")]
		public string Address { get; set; }

		/// <summary>
		/// Executes command invocation depending on 
		/// parameter set used.
		/// </summary>
		protected override void ProcessRecord()
		{
			var service = ServiceManager
				.Get<IHostFileDataService>();

			var entries = service.GetEntries();

			// ensure host exists before updating.
			if (!entries.Any(x => x.Hostname.AreHostFileStringEqual(Hostname)))
				throw new MissingHostException(Hostname);

			// locate and update entry.
			var toChange = entries.Single(x => x.Hostname.AreHostFileStringEqual(Hostname));
			var oldState = new HostFileEntry()
			{
				Address = toChange.Address,
				Hostname = toChange.Hostname,
				Type = toChange.Type
			};

			// udpdate address...
			toChange.Address = Address;
			var unchanged = entries.Where(x => !x.Hostname.AreHostFileStringEqual(Hostname));

			var composite = new List<HostFileEntry>();
			composite.AddRange(unchanged);
			composite.Add(toChange);

			// write all back out to disk.
			service.WriteEntries(composite);

			Log.WriteLog($"Updated host file entry: {oldState.ToString()} to {toChange.ToString()}");

			// END FUNCTION
		}

		// END CLASS (SetHostFileHostAddress)
	}

	// END NAMESPACE
}
