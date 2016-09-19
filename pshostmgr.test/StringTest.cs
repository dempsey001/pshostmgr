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
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManageHosts.Test
{
	using Utility;

	[TestClass]
	public class StringTest
	{
		[TestMethod]
		public void StringExt_HostStringComparisonShouldBeCurrentCultureIgnoreCase()
		{
			//
			// Other tests will be based on this assumption so just
			// verify this is still the case in code.
			//
			Assert.AreEqual(
				StringComparison.CurrentCultureIgnoreCase,
				StringExtensions.HostFileStringComparison);

			// END FUNCTION
		}

		[TestMethod]
		public void StringExt_ShouldCorrectlyCompareHostFileStrings()
		{
			// ensure thread culture setup to use en-US since that is
			// how the test is written.
			CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = ci;
			System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

			Assert.IsTrue(StringExtensions.AreHostFileStringEqual("abc", "ABC"));
			Assert.IsTrue(StringExtensions.AreHostFileStringEqual("abc", "AbC"));
			Assert.IsFalse(StringExtensions.AreHostFileStringEqual("abd", "abc"));

			var s = string.Intern("__interned_string");
			Assert.IsTrue(StringExtensions.AreHostFileStringEqual(s, s));

			// END FUNCTION
		}

		[TestMethod]
		public void StringExt_ShouldCallUsingExtension()
		{
			Assert.IsTrue("abc".AreHostFileStringEqual("ABC"));

			// END FUNCTION
		}

		// END CLASS (StringTest)
	}

	// END NAMESPACE
}
