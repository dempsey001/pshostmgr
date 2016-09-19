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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;

namespace ManageHosts.Test
{
	using Services;
	using Powershell.CmdLets;
	using Powershell;

	[TestClass]
	public class GetHostFileHostTest
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
		public void Should_GetAllHosts()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();

			ServiceManager.Provider = () => mySM;

			var entries = new[]
			{
				new HostFileEntry()
				{
					Address = "127.0.0.2",
					Hostname = "abc.com"
				},
				new HostFileEntry()
				{
					Address = "127.0.0.3",
					Hostname = "def.com"
				},
				new HostFileEntry()
				{
					Address = "127.0.0.4",
					Hostname = "ghi.com"
				}
			};

			mySM.SetupExistingHostList(entries);

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Get-HfHost");
			_powerShell.Commands = psCmd;
			var results = _powerShell.Invoke<HostFileRecord>();

			mySM.MockFileService.Verify(h => h.GetEntries());
			Assert.AreEqual(3, results.Count);
			Assert.IsTrue(results.Any( he =>
				string.Compare(he.Hostname, entries[0].Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
				string.Compare(he.Address, entries[0].Address, StringComparison.CurrentCultureIgnoreCase) == 0));
			Assert.IsTrue(results.Any(he =>
			   string.Compare(he.Hostname, entries[1].Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(he.Address, entries[1].Address, StringComparison.CurrentCultureIgnoreCase) == 0));
			Assert.IsTrue(results.Any(he =>
			   string.Compare(he.Hostname, entries[2].Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(he.Address, entries[2].Address, StringComparison.CurrentCultureIgnoreCase) == 0));

			// END FUNCTION
		}
		
		[TestMethod]
		public void Should_ReturnFilteredHosts()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			var entries = new[]
			{
				new HostFileEntry()
				{
					Address = "127.0.0.2",
					Hostname = "abc.com"
				},
				new HostFileEntry()
				{
					Address = "127.0.0.3",
					Hostname = "def.com"
				},
				new HostFileEntry()
				{
					Address = "127.0.0.4",
					Hostname = "ghi.com"
				}
			};

			mySM.SetupExistingHostList(entries);

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Get-HfHost");
			psCmd.AddParameter("Hostname", new[] { entries[0].Hostname, entries[2].Hostname });
			_powerShell.Commands = psCmd;
			var results = _powerShell.Invoke<HostFileRecord>();

			mySM.MockFileService.Verify(h => h.GetEntries());
			Assert.AreEqual(2, results.Count);
			Assert.IsTrue(results.Any(he =>
			   string.Compare(he.Hostname, entries[0].Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(he.Address, entries[0].Address, StringComparison.CurrentCultureIgnoreCase) == 0));
			Assert.IsTrue(results.Any(he =>
			   string.Compare(he.Hostname, entries[2].Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(he.Address, entries[2].Address, StringComparison.CurrentCultureIgnoreCase) == 0));

			// END FUNCTION
		}

		[TestMethod]
		public void Should_ReturnEmptyForMissingHost()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Get-HfHost");
			psCmd.AddParameter("Hostname", "foo.com");

			_powerShell.Commands = psCmd;
			var results = _powerShell.Invoke();

			Assert.AreEqual(0, results.Count);


			// END FUNCTION
		}

		// END CLASS (GetHostFileHostTest)
	}

	// END NAMESPACE
}