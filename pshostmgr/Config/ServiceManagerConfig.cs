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

namespace ManageHosts.Config
{
	using Services;
	
	/// <summary>
	/// Static class with code that performs module
	/// level service manager initialization.
	/// </summary>
	internal static class ServiceManagerConfig
	{
		// where in syslog to look for event data.
		internal const string SystemLogEventSource = "ManageHosts CmdLets";
		
		/// <summary>
		/// Registers requried interfaces with their
		/// relevant implementation objects.
		/// </summary>
		public static void Initialize()
		{
			if (!ServiceManager.IsRegistered< IHostFileDataService>())
				ServiceManager.Register<IHostFileDataService, HostsFileService>();

			// using type initializer to set the event source.
			if (!ServiceManager.IsRegistered<ILoggingService>())
			{
				ServiceManager.Register<ILoggingService, SysEventLog>( (inf) =>
				{
					(inf as SysEventLog).EventSource = SystemLogEventSource;
					return inf;
				});
			}

			// END FUNCTION
		}

		// END CLASS (ServiceManagerConfig)
	}

	// END NAMESPACE
}
