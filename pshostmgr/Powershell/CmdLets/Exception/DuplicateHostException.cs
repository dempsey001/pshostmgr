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
using ManageHosts.Utility;

namespace ManageHosts.Powershell.CmdLets
{
	/// <summary>
	/// Exception for when an operation takes place
	/// against a duplicate host entry.
	/// </summary>
	public class DuplicateHostException : Exception
	{
		/// <summary>
		/// Default ctor. 
		/// </summary>
		public DuplicateHostException(string hostname)
			: base($"Duplicate host was detected. Entry: {hostname ?? "<empty>"}. " +
			       $"Use Set-HfHostDestination instead.")
		{
			Verify.NotEmpty(hostname, nameof(hostname));

            Host = hostname;

			// END FUNCTION	
		}

	    /// <summary>
	    /// Returns hostname associated with exception.
	    /// </summary>
	    public string Host { get; }

	    // END CLASS (DuplicateHostException)
	}
	
	// END NAMESPACE
}
