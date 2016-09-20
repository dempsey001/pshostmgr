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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManageHosts.Test
{
	using Services;

	[TestClass]
	public class ServiceManagerTest
	{
		// Simple test interface.
		private interface IFoo { string GetFoo(); }

		// Concrete for foo.
		private class Foo : IFoo
		{ public string GetFoo() { return "foo"; } }
		
		// dervives and adds bar.
		private interface IBar : IFoo { string GetBar(); }

		// implements IBar.
		private class FooBar : IBar
		{
			public string GetFoo() { return "foo"; }
			public string GetBar() { return "bar"; }
		}

		// unimplemented interface to test againt.
		private interface INoImpl { string NotImplMethod(); }
		
		[TestMethod]
		public void Should_DetectRegisteredServiceType()
		{
			ServiceManager.Provider = () => new ServiceManager();
			ServiceManager.Register<IFoo,Foo>();

			Assert.IsTrue(ServiceManager.IsRegistered<IFoo>());

			// END FUNCTION
		}

		[TestMethod]
		public void Should_DetectUnegisteredServiceType()
		{
			Assert.IsFalse(ServiceManager.IsRegistered<INoImpl>());
			
			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(ServiceNotFoundException))]
		public void Should_ThrowIfFetchingUnregisteredType()
		{
			ServiceManager.Provider = () => new ServiceManager();
			ServiceManager.Get<INoImpl>();

			// END FUNCTION
		}

		[TestMethod, ExpectedException(typeof(Exception))]
		public void Should_ThrowOnDuplicateTypeRegistration()
		{
			ServiceManager.Provider = () => new ServiceManager();
			ServiceManager.Register<IFoo, Foo>();
			ServiceManager.Register<IFoo, Foo>();

			// END FUNCTION
		}

		[TestMethod]
		public void Should_ReturnCorrectTypeForRegisteredInterface()
		{
			ServiceManager.Provider = () => new ServiceManager();
			ServiceManager.Register<IFoo, Foo>();

			Assert.AreEqual(typeof(Foo),
				ServiceManager.Get<IFoo>().GetType());

			// END FUNCTION
		}

		// END CLASS (ServiceManagerTest)
	}

	// END NAMESPACE
}
