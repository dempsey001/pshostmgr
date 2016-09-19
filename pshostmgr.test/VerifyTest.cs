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
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManageHosts.Test
{
	using Utility;

	[TestClass]
	public class VerifyTest
	{
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void NotNull_ShouldThrowOnNull()
		{
			Verify.NotNull(null, "test");

			// END FUNCTION
		}

		[TestMethod]
		public void NotNull_ShouldDoNothingOnNonNull()
		{
			Verify.NotNull(new object(), "test");

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void NotEmpty_ShouldThrowOnEmptyString()
		{
			Verify.NotEmpty("", "test");

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void NotEmpty_ShouldThrowOnNullString()
		{
			string s = null;
			Verify.NotEmpty(s, "test");

			// END FUNCTION
		}

		[TestMethod]
		public void NotEmpty_ShouldDoNothingOnNonNull()
		{
			Verify.NotEmpty("abc", "test");

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void NotEquals_ShouldThrowOnEqualObjects()
		{
			Verify.NotEquals(2, 2, "test");

			// END FUNCTION
		}

		[TestMethod]
		public void NotEquals_ShouldDoNothingOnUnequalObjects()
		{
			Verify.NotEquals(2, 3, "test");

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(FileNotFoundException))]
		public void FileExists_ShouldThrowIfFileDoesNotExist()
		{
			var fn = @"C:\Windows\system32\drivers\willywonka\choco.factory.exe";
			Assert.IsTrue(!File.Exists(fn)); // just for sanity.
			Verify.FileExists(fn);

			// END FUNCTION
		}

		[TestMethod]
		public void FileExists_ShouldDoNothingIfFileExist()
		{
			var fn = Path.GetTempFileName();
			try
			{
				// make sure it exists on disk.
				using (var w = new StreamWriter(new FileStream(fn, FileMode.OpenOrCreate)))
					w.Close();

				Assert.IsTrue(File.Exists(fn));
				Verify.FileExists(fn);
			}
			finally
			{
				if (File.Exists(fn))
					File.Delete(fn);
			}

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void FileNotPresent_ShouldThrowIfFileExist()
		{
			var fn = Path.GetTempFileName();
			try
			{
				// make sure it exists on disk.
				using (var w = new StreamWriter(new FileStream(fn, FileMode.OpenOrCreate)))
					w.Close();

				Assert.IsTrue(File.Exists(fn));
				Verify.FileNotPresent(fn);
			}
			finally
			{
				if (File.Exists(fn))
					File.Delete(fn);
			}

			// END FUNCTION
		}

		[TestMethod]
		public void FileNotPresent_ShouldDoNothingIfFileDoesNotExist()
		{
			var fn = @"C:\Windows\system32\drivers\willywonka\choco.factory.exe";
			Assert.IsTrue(!File.Exists(fn)); // just for sanity.
			Verify.FileNotPresent(fn);

			// END FUNCTION
		}

		// END CLASS (VerifyTest)
	}

	// END NAMESPACE 
}
