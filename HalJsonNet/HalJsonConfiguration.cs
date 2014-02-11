using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalJsonNet.Utility;

namespace HalJsonNet
{
	public class HalJsonConfiguration
	{
		private readonly ThreadSafeCache<Type, HalJsonTypeConfiguration> _config = new ThreadSafeCache<Type, HalJsonTypeConfiguration>();

	    public HalJsonConfiguration(string urlBase = null)
	    {
	        UrlBase = urlBase;
	    }

	    public string UrlBase { get; private set; }

        

		public bool TryGetTypeConfiguration(Type type, out HalJsonTypeConfiguration config)
		{
			return _config.TryGet(type, out config);
		}

		public HalJsonTypeConfiguration GetOrCreateTypeConfiguration(Type type)
		{
			return _config.GetOrAdd(type,
				t => (HalJsonTypeConfiguration) Activator.CreateInstance(typeof (HalJsonTypeConfiguration<>).MakeGenericType(t)));
		}

		public HalJsonTypeConfiguration<T> Configure<T>()
		{
			return (HalJsonTypeConfiguration<T>) GetOrCreateTypeConfiguration(typeof (T));
		}
	}
}
