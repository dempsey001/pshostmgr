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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Management.Automation;
using System.Collections.Generic;

namespace ManageHosts.Test
{
	using Services;
	using Powershell.CmdLets;
	
	[TestClass]
	public class RemoveHostFileHostTest
	{
		private PowerShell _powerShell;

		[TestInitialize]
		public void RemHostFileInit()
		{
			_powerShell = PowerShell.Create();
			_powerShell
				.AddCommand("Import-Module")
				.AddParameter("Assembly", 
				typeof(AddHostFileHost).Assembly);
			_powerShell.Invoke();


			// END FUNCTION
		}

		[TestMethod]
		public void Should_RemoveCorrectHostDataFromHostFile()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();

			ServiceManager.Provider = () => mySM;

			var hfe = new HostFileEntry()
			{
				Address = "127.0.0.2",
				Hostname = "abc.com"
			};

			mySM.SetupExistingHostList(new [] { hfe });

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Remove-HfHost");
			psCmd.AddParameter("Hostname", hfe.Hostname);
			
			_powerShell.Commands = psCmd;
			_powerShell.Invoke();

			mySM.MockFileService.Verify(h => h.WriteEntries(
				It.Is<IEnumerable<HostFileEntry>>(en => !en.Any(writeEntry => 
				string.Compare(writeEntry.Hostname, hfe.Hostname) == 0 &&
				string.Compare(writeEntry.Address, hfe.Address) == 0 ))));

			// END FUNCTION
		}
		
		[TestMethod, ExpectedException(typeof(MissingHostException))]
		public void Should_ThrowOnUnknownHost()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();

			ServiceManager.Provider = () => mySM;

			var hfe = new HostFileEntry()
			{
				Address = "127.0.0.2",
				Hostname = "abc.com"
			};

			mySM.SetupExistingHostList(new[] { hfe });

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Remove-HfHost");
			psCmd.AddParameter("Hostname", hfe.Hostname + "aa");

			try
			{
				_powerShell.Commands = psCmd;
				_powerShell.Invoke();
			}
			catch (CmdletInvocationException cex)
			{
				throw cex.InnerException ?? cex;
			}

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ParameterBindingException))]
		public void Should_ThrowOnMissingHostName()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Remove-HfHost");
			_powerShell.Commands = psCmd;
			_powerShell.Invoke();


			// END FUNCTION
		}

		// END CLASS (RemoveHostFileHostTest)
	}

	// END NAMESPACE
}