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
using System.Collections.Generic;
using TinyIoC;

namespace ManageHosts.Services
{
	/// <summary>
	/// Service manager singleton to provide injectable
	/// services for application use.
	/// </summary>
	public class ServiceManager
	{
        #region Static & NonStatic Fields
        // singleton reference.
        private static ServiceManager _field;

		// field to hold creator func for derived type creation.
		private static Func<ServiceManager> _provider;

		// our IOC container.
		private readonly TinyIoCContainer _container = new TinyIoCContainer();

		private readonly Dictionary<Type, object> _inits 
			= new Dictionary<Type, object>();
        #endregion

        /// <summary>
        /// Accessor that defines how the service manager
        /// is created. This is mostly helpful during 
        /// test module creation to allow service manager
        /// instance override.
        /// </summary>
        public static Func<ServiceManager> Provider
		{
			get { return _provider ?? (() => new ServiceManager()); }
			set
			{
				_provider = value;
				// cause recreation of SM from new provider.
				_field = null; 
			}

			// END ACCESSOR (Provider)
		}

		/// <summary>
		/// Retrieves the existing service manager or creates
		/// and returns a new only if one is not created.
		/// </summary>
		private static ServiceManager _instance => 
			(_field ?? (_field = Provider?.Invoke()));

		/// <summary>
		/// Registers a single service type.
		/// </summary>
		/// <typeparam name="TInterface">Service interface type</typeparam>
		/// <typeparam name="TConcrete">Concrete type implementing that interface.</typeparam>
		/// <param name="initializer">Optional type initializer for interface.</param>
		protected virtual void RegisterService<TInterface, TConcrete>(Func<TInterface, TInterface> initializer)
			where TInterface : class
			where TConcrete : class, TInterface
		{
			if (!typeof(TInterface).IsInterface)
				throw new Exception("Service type must be an interface.");
			if (typeof(TConcrete).IsInterface || typeof(TConcrete).IsAbstract)
				throw new Exception("Implementation object must be concrete.");
			if (_container.CanResolve<TInterface>())
			{
				throw new Exception(
					"Interface already registered. Duplicate " +
					"resolution types are not allowed.");
			}

			_container.Register<TInterface, TConcrete>();
			if (null != initializer) {
				_inits.Add(typeof(TInterface), initializer);
			}
			
			// END FUNCTION
		}

		/// <summary>
		/// Passes through to the current instance of the 
		/// service manager registration function.
		/// </summary>
		/// <typeparam name="TInterface">Service interface type</typeparam>
		/// <typeparam name="TConcrete">Concrete type implementing that interface.</typeparam>
		/// <param name="initializer">Optional type initializer for interface.</param>
		public static void Register<TInterface, TConcrete>(Func<TInterface, TInterface> initializer = null)
			where TInterface : class
			where TConcrete : class, TInterface
		{
			_instance.RegisterService<TInterface, TConcrete>(initializer);

			// END FUNCTION
		}

		/// <summary>
		/// Looks up the desired service in our service map
		/// and returns the registered object.
		/// </summary>
		/// <typeparam name="TInterface">Type of service to retrieve</typeparam>
		/// <returns>They desired service object if found, else false.</returns>
		protected virtual TInterface GetService<TInterface>() where TInterface : class
		{
			try
			{
				var objInf = _container.Resolve<TInterface>();
				if (null != objInf && _inits.ContainsKey(typeof(TInterface)))
				{
					Func<TInterface,TInterface> inilzr = _inits[typeof(TInterface)]
						as Func<TInterface, TInterface>;
					objInf = inilzr?.Invoke(objInf) ?? objInf;
				}
				return objInf;
			}
			catch (TinyIoCResolutionException iocEx)
			{
				throw new ServiceNotFoundException(typeof(TInterface), iocEx);
			}

		    // END FUNCTION
		}

		/// <summary>
		/// Passes through to the current instance of the 
		/// service manager retrieval function.
		/// </summary>
		/// <typeparam name="TInterface">Type of service to retrieve</typeparam>
		/// <returns>They desired service object if found, else false.</returns>
		public static TInterface Get<TInterface>() where TInterface : class
		{
			return _instance.GetService<TInterface>();

			// END FUNCTION
		}

		/// <summary>
		/// Tests whether a given service has been registered
		/// for a provided interface type.
		/// </summary>
		/// <typeparam name="TInterface">Type to check registration for.</typeparam>
		/// <returns>True if registered, else false.</returns>
		public static bool IsRegistered<TInterface>() where TInterface : class
		{
			return _instance._container?.CanResolve<TInterface>() ?? false;

			// END FUNCTION
		}

		// END CLASS (ServiceManager)
	}

	// END NAMESPACE
}
