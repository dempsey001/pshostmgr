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
using Moq;
using System.Management.Automation;
using System.Collections.Generic;

namespace ManageHosts.Test
{
	using Services;
	using Powershell.CmdLets;
	
	[TestClass]
	public class AddHostFileHostTest
	{
		private PowerShell _powerShell;

		[TestInitialize]
		public void AddHostFileInit()
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
		public void Should_AddCorrectHostDataToHostFile()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();

			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Hostname", "abc.com");
			psCmd.AddParameter("Address", "127.0.0.2");
			
			_powerShell.Commands = psCmd;
			_powerShell.Invoke();

			mySM.MockFileService.Verify(h => h.WriteEntries(
				It.Is<IEnumerable<HostFileEntry>>(en => en.Any(hfe => 
				string.Compare(hfe.Hostname, "abc.com") == 0 &&
				string.Compare(hfe.Address, "127.0.0.2") == 0 ))));

			// END FUNCTION
		}

		[TestMethod]
		public void Should_ReturnHostEntry()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Hostname", "abc.com");
			psCmd.AddParameter("Address", "127.0.0.2");

			_powerShell.Commands = psCmd;
			var results = _powerShell.Invoke<HostFileEntry>();

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("abc.com", results.First().Hostname);
			Assert.AreEqual("127.0.0.2", results.First().Address);


			// END FUNCTION
		}

		[TestMethod]
		public void Should_KeepExistingEntries()
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
				}
			};

			mySM.SetupExistingHostList(entries);

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Hostname", "xyz.com");
			psCmd.AddParameter("Address", "127.0.0.4");

			_powerShell.Commands = psCmd;
			var results = _powerShell.Invoke<HostFileEntry>();

			Assert.IsNotNull(results);
			//Assert.AreEqual(3, results.Count);


			mySM.MockFileService.Verify(h => h.WriteEntries(
				It.Is<IEnumerable<HostFileEntry>>(en => en.Any(writeEntry =>
					(string.Compare(entries[0].Hostname, writeEntry.Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare(entries[0].Address, writeEntry.Address, StringComparison.CurrentCultureIgnoreCase) == 0)
				|| (string.Compare(entries[1].Hostname, writeEntry.Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare(entries[1].Address, writeEntry.Address, StringComparison.CurrentCultureIgnoreCase) == 0)
				|| (string.Compare("xyz.com", writeEntry.Hostname, StringComparison.CurrentCultureIgnoreCase) == 0 &&
					string.Compare("127.0.0.4", writeEntry.Address, StringComparison.CurrentCultureIgnoreCase) == 0)))));


			// END FUNCTION
		}


		[TestMethod, ExpectedException(typeof(DuplicateHostException))]
		public void Should_ThrowOnDuplicate()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Hostname", "abc.com");
			psCmd.AddParameter("Address", "127.0.0.2");

			PSCommand psDup = new PSCommand();
			psDup.AddCommand("Add-HfHost");
			psDup.AddParameter("Hostname", "abc.com");
			psDup.AddParameter("Address", "127.0.0.2");

			try
			{
				_powerShell.Commands = psCmd;
				var result = _powerShell.Invoke<HostFileEntry>();

				mySM.MockFileService.Setup(fs => fs.GetEntries()).Returns(
					new List<HostFileEntry> { result.First() });

				_powerShell.Commands.Clear();

				_powerShell.Commands = psDup;
				_powerShell.Invoke();
			}
			catch (CmdletInvocationException cex)
			{
				// testing for what the cmdlet itself actually threw.
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
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Address", "127.0.0.2");
			_powerShell.Commands = psCmd;
			_powerShell.Invoke();


			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ParameterBindingException))]
		public void Should_ThrowOnMissingAddress()
		{
			_powerShell.Commands.Clear();
			var mySM = new MockServiceManager();
			ServiceManager.Provider = () => mySM;

			PSCommand psCmd = new PSCommand();
			psCmd.AddCommand("Add-HfHost");
			psCmd.AddParameter("Hostname", "abc.com");
			_powerShell.Commands = psCmd;
			_powerShell.Invoke();

			// END FUNCTION
		}


		// END CLASS (AddHostFileHostTest)
	}



	// END NAMESPACE
}