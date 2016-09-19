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
using Moq;
using System.Collections.Generic;

namespace ManageHosts.Test
{
	using Services;

	/// <summary>
	/// Provides a basic service manager that serves
	/// mock service objects instead of real ones.
	/// </summary>
	internal sealed class MockServiceManager : ServiceManager
	{
		public Mock<IHostFileDataService> MockFileService = 
			new Mock<IHostFileDataService>();

		// we don't actually do anything for registrations.
		protected override void RegisterService<TInterface, TConcrete>(Func<TInterface, TInterface> initializer) { }

		// provide a dead log and our mock for the host file service.
		protected override T GetService<T>()
		{
			if (typeof(T) == typeof(ILoggingService))
				return new NullLog() as T;
			if (typeof(T) == typeof(IHostFileDataService))
				return MockFileService.Object as T;
			return null;
		}

		/// <summary>
		/// Prepares the host file interface with precanned
		/// data to return from the mock.
		/// </summary>
		/// <param name="entries">The entries to serve up.</param>
		public void SetupExistingHostList(IEnumerable<HostFileEntry> entries)
		{
			MockFileService.Setup(fs => fs.GetEntries()).Returns(entries);

			// END FUNCTION
		}

		// END CLASS (MockServiceManager)
	}


	// END NAMESPACE
}
