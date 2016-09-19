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
using System.Linq;
using System.Management.Automation;

namespace ManageHosts.Powershell.CmdLets
{
	using Utility;
	using Services;

	/// <summary>
	/// Provies cmdlet to remove a host 
	/// </summary>
	[Cmdlet(VerbsCommon.Remove, "HfHost")]
	public sealed class RemoveHostFileHost : ServiceSupportedCmdLet
	{
		/// <summary>
		/// A new entry must be given a name. This is
		/// a mandatory parameter.
		/// </summary>
		[Parameter(
			Mandatory			= true, 
			HelpMessage			= "Enter a hostname ",
			Position			= 0)]
		[Alias("Host")]
		public string Hostname { get; set; }

		/// <summary>
		/// Executes invocation of deletion. Verifies record exists
		/// prior to deletion.
		/// </summary>
		protected override void ProcessRecord()
		{
			var service = ServiceManager
				.Get<IHostFileDataService>();

			var entries = service.GetEntries();
			try
			{
				var entry = entries.Single(x =>
					x.Hostname.AreHostFileStringEqual(Hostname));

				var toWrite = entries.Where(x =>
					!x.Hostname.AreHostFileStringEqual(Hostname))
					.ToList();

				service.WriteEntries(toWrite);

				Log.WriteLog($"Removed host file entry: {entry.ToString()}");
			}
			catch (InvalidOperationException)
			{
				// thrown by single if not found.
				throw new MissingHostException(Hostname);
			}

			// END FUNCTION
		}

		// END CLASS (RemoveHostFileHost)
	}

	// END NAMESPACE
}
