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

namespace ManageHosts.Utility
{
	/// <summary>
	/// Static string helper extensions used throughout
	/// this module.
	/// </summary>
	internal static class StringExtensions
	{
		public static StringComparison HostFileStringComparison 
			= StringComparison.CurrentCultureIgnoreCase;

		// performs comparison of objects first then moves to 
		// string comparison of requested type.
		private static bool RefCheckCompareWithType(string a, string b, StringComparison compT)
		{
			if (a == null && b == null)
				return true;
			if (object.ReferenceEquals(a, b))
				return true;
			if ((null == a && b != null) || (a != null && b == null))
				return false;
			return string.Compare(a, b, compT) == 0;
		}

		/// <summary>
		///	Compares two string in a manner consistent with how
		/// we want to compare string in the host file.
		/// </summary>
		/// <param name="a">string to compare.</param>
		/// <param name="a">string to compare to</param>
		public static bool AreHostFileStringEqual(this string a, string b)
		{
			return RefCheckCompareWithType(a, b, HostFileStringComparison);
		}

		// END CLASS (StringExtensions)
	}

	// END NAMESPACE
}
