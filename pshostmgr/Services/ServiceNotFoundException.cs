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

namespace ManageHosts.Services
{
	/// <summary>
	/// Basic exception object for when a requested service could
	/// not be located/resolved.
	/// </summary>
	public class ServiceNotFoundException : Exception
	{
		/// <summary>
		/// Default ctor. Notes the missing service.
		/// </summary>
		public ServiceNotFoundException(Type srvType)
			: base($"Failed to locate registered service for type: ${srvType}")
		{
            ServiceType = srvType;

			// END FUNCTION	
		}

		/// <summary>
		/// Extra ctor taking inner exception.
		/// </summary>
		public ServiceNotFoundException(Type srvType, Exception inner) 
			: base($"Failed to locate registered service for type: ${srvType}", inner)
		{
			ServiceType = srvType;

			// END FUNCTION	
		}

		/// <summary>
		/// Returns hostname associated with exception.
		/// </summary>
		public Type ServiceType { get; }

        // END CLASS (ServiceNotFoundException)
    }

    // END NAMESPACE
}
