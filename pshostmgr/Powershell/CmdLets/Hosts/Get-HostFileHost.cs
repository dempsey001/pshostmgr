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
using System.Management.Automation;

namespace ManageHosts.Powershell.CmdLets
{
	using Services;
	using Utility;

	/// <summary>
	/// Cmdlet object for geting host information 
	/// </summary>
	[Cmdlet(VerbsCommon.Get, "HfHost")]
	public sealed class GetHostFileHost : ServiceSupportedCmdLet
	{
		/// <summary>
		/// A new entry must be given a name. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(Position = 0), Alias("Host")]
		public string[] Hostname { get; set; }

		/// <summary>
		/// Executes command invocation depending on 
		/// parameter set used.
		/// </summary>
		protected override void ProcessRecord()
		{
			var service = ServiceManager
				.Get<IHostFileDataService>();

			var entries = service?.GetEntries();

			// check to see if we should filter this list.
			if (null != Hostname && Hostname.Length > 0)
				entries = entries.Where(x => Hostname.Any(y => x.Hostname.AreHostFileStringEqual(y)));

			entries.Select(x => 
				new HostFileRecord()
				{
					Hostname = x.Hostname,
					Address = x.Address
				})
				.ToList()
				.ForEach(WriteObject);

			// END FUNCTION
		}

		// END CLASS (GetHostFileHost)
	}

	// END NAMESPACE
}
