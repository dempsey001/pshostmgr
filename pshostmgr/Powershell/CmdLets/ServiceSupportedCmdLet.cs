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
using System.Management.Automation;

namespace ManageHosts.Powershell
{
	using Config;
	using Services;

	/// <summary>
	/// Simple base object to ensure service manager
	/// has been initialized prior to executing a cmdlet
	/// that relies on its provided services.
	/// </summary>
	public class ServiceSupportedCmdLet : PSCmdlet
	{
		/// <summary>
		/// Ensures service manager is ready for use.
		/// </summary>
		protected override void BeginProcessing()
		{
			ServiceManagerConfig.Initialize();
			
			// END FUNCTION
		}

		/// <summary>
		/// Nothing to do here.
		/// </summary>
		protected override void EndProcessing()
		{
			// END FUNCTION
		}

		//
		// quick service accessors. These just handle
		// querying the SM.
		//

		/// <summary>
		/// Returns the logging service.
		/// </summary>
		internal ILoggingService Log => ServiceManager.Get<ILoggingService>();

		/// <summary>
		/// Returns the host file service.
		/// </summary>
		internal IHostFileDataService HostFileService => ServiceManager.Get<IHostFileDataService>();

		// END CLASS (ServiceSupportedCmdLet)
	}

	// END NAMESPACE
}
