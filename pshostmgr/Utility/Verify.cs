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
using System.IO;

namespace ManageHosts.Utility
{
	/// <summary>
	///     Static class used to perform verification options upon
	///     input data for various methods. This simplifies the interface
	///     for checking parameters and throwing exceptions.
	/// </summary>
	internal static class Verify
	{
		/// <summary>
		///     Verifies that the passed in parameter is not null. If the
		///     param is null it causes an exception to be thrown.
		/// </summary>
		/// <param name="obj">Object to test.</param>
		/// <param name="paramName">Name of parameter as it was in the calling method.</param>
		public static void NotNull(object obj, string paramName)
		{
			if ( null == obj )
			{
				throw new ArgumentNullException(
					$"Parameter, {paramName}, was null. Illegal value.", paramName );
			}

			// END FUNCTION
		}

		/// <summary>
		///     Verifies that the passed in parameter is an empty string. If the
		///     param is empty it causes an exception to be thrown.
		/// </summary>
		/// <param name="str">Object to test.</param>
		/// <param name="paramName">Name of parameter as it was in the calling method.</param>
		public static void NotEmpty(string str, string paramName)
		{
			if ( String.IsNullOrEmpty( str ) )
			{
				throw new ArgumentException(
					$"Parameter, {paramName}, was null or an empty string. Illegal value.", paramName );
			}

			// END FUNCTION
		}

		/// <summary>
		///     Compares two values to ensure the parameter does NOT
		///     equal the passed in comparison value.
		/// </summary>
		/// <param name="obj">Paramter to compare.</param>
		/// <param name="compSrc">Value obj should not be equal to.</param>
		/// <param name="paramName">Name of the paramter in the calling method.</param>
		public static void NotEquals(object obj, object compSrc, string paramName)
		{
			if ( obj.Equals( compSrc ) )
			{
				throw new ArgumentException(
					$"Parameter, {paramName}, fails inequality test against {compSrc ?? "null"}." );
			}

			// END FUNCTION
		}

		/// <summary>
		///     Verifies that the passed is a path to a valid file.
		/// </summary>
		/// <param name="path">Path to file to test for.</param>
		public static void FileExists(string path)
		{
			NotEmpty( path, nameof( path ) );
			if ( !File.Exists( path ) )
			{
				throw new FileNotFoundException(
					$"File path provided did not point to a valid path. Location: {path}", path );
			}

			// END FUNCTION
		}

		/// <summary>
		///     Verifies that the passed in parameter is not null. If the
		///     param is null it causes an exception to be thrown.
		/// </summary>
		/// <param name="path">Path to file to test for.</param>
		public static void FileNotPresent(string path)
		{
			if ( File.Exists( path ) )
			{
				throw new ArgumentException(
					$"File path provided already exists. File system object may not exist at: {path}" );
			}

			// END FUNCTION
		}

		// END CLASS (Verify)
	}

	// END NAMESPACE
}
