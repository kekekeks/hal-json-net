using System;
using HalJsonNet.Configuration;
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
			var rv = _config.TryGet(type, out config);
		    if (!rv && AttributeConfigurationResolver.GetConfigurationOrNull(type) != null) // No configuration in cache, but found attribute-based one
		    {
		        config = GetOrCreateTypeConfiguration(type);
		        return true;
		    }
		    return rv;
		}

		public HalJsonTypeConfiguration GetOrCreateTypeConfiguration(Type type)
		{
			return _config.GetOrAdd(type,
				t => AttributeConfigurationResolver.GetConfigurationOrNull(type) ?? (HalJsonTypeConfiguration) Activator.CreateInstance(typeof (HalJsonTypeConfiguration<>).MakeGenericType(t)));
		}

		public HalJsonTypeConfiguration<T> Configure<T>()
		{
			return (HalJsonTypeConfiguration<T>) GetOrCreateTypeConfiguration(typeof (T));
		}
	}
}
