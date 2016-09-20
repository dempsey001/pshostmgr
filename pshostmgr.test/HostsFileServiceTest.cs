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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManageHosts.Test
{
	using System.Text.RegularExpressions;
	using Services;

	[TestClass]
	public class HostsFileServiceTest
	{
		[TestMethod]
		public void Should_EnumerateCorrectEntries()
		{
			HostsFileService hfs = new HostsFileService()
			{
				HostFilePath = Path.GetTempFileName()
			};

			using (var w = new StreamWriter(
				new FileStream(hfs.HostFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
			{
				w.WriteLine("# fdafdasf");
				w.WriteLine("# fdfdsafdafdasfdsafas");
				w.WriteLine("#");
				w.WriteLine("");
				w.WriteLine("10.0.1.2                                        abc.com");
				w.WriteLine("255.122.111.56\t  def.com");
				w.WriteLine("# 255.122.111.46\tdontfindme.com");
				w.WriteLine("");
				w.WriteLine("# ::56\t\t      v6.com");
				w.WriteLine("::1\tlocalhost");
				w.WriteLine("");
				w.WriteLine("2001:0db8:85a3:0000:0000:8a2e:0370:7334    \tbigv6.net");
			}

			var entries = hfs.GetEntries();
			Assert.AreEqual(4, entries.Count());

			Assert.IsTrue(entries.Any(e =>
			   string.Compare(e.Hostname, "abc.com", StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(e.Address, "10.0.1.2", StringComparison.CurrentCultureIgnoreCase) == 0));

			Assert.IsTrue(entries.Any(e =>
			   string.Compare(e.Hostname, "def.com", StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(e.Address, "255.122.111.56", StringComparison.CurrentCultureIgnoreCase) == 0));

			Assert.IsTrue(entries.Any(e =>
			   string.Compare(e.Hostname, "localhost", StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(e.Address, "::1", StringComparison.CurrentCultureIgnoreCase) == 0));

			Assert.IsTrue(entries.Any(e =>
			   string.Compare(e.Hostname, "bigv6.net", StringComparison.CurrentCultureIgnoreCase) == 0 &&
			   string.Compare(e.Address, "2001:0db8:85a3:0000:0000:8a2e:0370:7334", StringComparison.CurrentCultureIgnoreCase) == 0));

			try { File.Delete(hfs.HostFilePath); } catch { }

			// END FUNCTION
		}

		[TestMethod]
		public void Should_WriteNoEntries()
		{
			var hfs = new HostsFileService()
			{
				HostFilePath = Path.GetTempFileName()
			};

			hfs.WriteEntries(new HostFileEntry[] { });

			Assert.AreEqual(0, hfs.GetEntries().Count());

			try { File.Delete(hfs.HostFilePath); } catch { }

			// END FUNCTION
		}

		[TestMethod]
		public void Should_WriteCorrectEntries()
		{
			var hfs = new HostsFileService()
			{
				HostFilePath = Path.GetTempFileName()
			};

			hfs.WriteEntries(new [] {
				new HostFileEntry() { Address = "::1", Hostname = "localhost" },
				new HostFileEntry() { Address = "10.1.44.19", Hostname = "sqlserver.domain.net" },
				new HostFileEntry() { Address = "2001:0db8:85a3:0000:0000:8a2e:0370:7334", Hostname = "bigv6.net" }
			});

			using (var r = new StreamReader(
				new FileStream(hfs.HostFilePath, FileMode.Open, FileAccess.Read)))
			{
				var fileData = r.ReadToEnd();

				Assert.IsTrue(new Regex($@"\s*::1\s*localhost\s*{Environment.NewLine}").IsMatch(fileData));
				Assert.IsTrue(new Regex($@"\s*10.1.44.19\s*sqlserver.domain.net\s*{Environment.NewLine}").IsMatch(fileData));
				Assert.IsTrue(new Regex($@"\s*2001:0db8:85a3:0000:0000:8a2e:0370:7334\s*bigv6.net\s*{Environment.NewLine}").IsMatch(fileData));
			}

			try { File.Delete(hfs.HostFilePath); } catch { }

			// END FUNCTION
		}

		// END CLASS (HostsFileServiceTest)
	}



	// END NAMESPACE
}