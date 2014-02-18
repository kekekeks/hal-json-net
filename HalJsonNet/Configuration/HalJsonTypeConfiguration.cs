using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using HalJsonNet.Utility;

namespace HalJsonNet.Configuration
{
	public class HalJsonTypeConfiguration
	{
		private ImmutableDictionary<string, Link> _links = ImmutableDictionary<string, Link>.Empty;
		private ImmutableHashSet<PropertyInfo> _hiddenProperties = ImmutableHashSet<PropertyInfo>.Empty;

		private ImmutableDictionary<string, Embedded> _embedded =
			ImmutableDictionary<string, Embedded>.Empty;
		private object _syncRoot = new object();

		public IReadOnlyDictionary<string, Link> Links { get { return _links; } }
		public IEnumerable<PropertyInfo> HiddenProperties { get { return _hiddenProperties; } }
		public IReadOnlyDictionary<string, Embedded> Embedded { get { return _embedded; } }

		internal void HideProperty(PropertyInfo nfo)
		{
			lock (_syncRoot)
				_hiddenProperties = _hiddenProperties.Add(nfo);
		}
		public HalJsonTypeConfiguration Link (string name, Link link)
		{
			lock (_syncRoot)
				_links = _links.SetItem (name, link);
			return this;
		}

		public HalJsonTypeConfiguration Link (string name, string link, bool templated = false)
		{
			return Link (name, new Link (_ => link, templated));
		}

		internal HalJsonTypeConfiguration Embed(string name, Func<object, object> getter)
		{
			lock (_syncRoot)
				_embedded = _embedded.SetItem(name, new Embedded(getter));
			return this;
		}

	    public HalJsonTypeConfiguration Clone()
	    {
	        lock (_syncRoot)
	        {
	            var rv = (HalJsonTypeConfiguration) MemberwiseClone();
	            rv._syncRoot = new object();
	            return rv;
	        }
	    }

	}

	public sealed class HalJsonTypeConfiguration<T> : HalJsonTypeConfiguration
	{
		public new HalJsonTypeConfiguration<T> Link (string name, Link link)
		{
			base.Link(name, link);
			return this;
		}

		public new HalJsonTypeConfiguration<T> Link(string name, string link, bool templated = false)
		{
			base.Link(name, link, templated);
			return this;
		}

		public HalJsonTypeConfiguration<T> Link(string name, Func<T, string> getter, bool templated = false)
		{
			return Link(name, new Link(o => getter((T) o), templated));
		}

		public HalJsonTypeConfiguration<T> Link<TProp>(string name, Expression<Func<T, TProp>> prop,
			Func<TProp, string> linkGetter, bool hideProperty, bool templated = false)
		{
			var p = ReflectionUtility.ExtractProperty(prop);
			var getter = ReflectionUtility.CreateDelegate<Func<T, TProp>>(p.GetMethod, null);
			if (hideProperty)
				HideProperty(p);
			return Link(name, o => linkGetter(getter(o)), templated);
		}

		public HalJsonTypeConfiguration<T> Embed<TProp>(string name, Func<T, TProp> getter)
		{
			base.Embed(name, o => getter((T) o));
			return this;
		}

		public HalJsonTypeConfiguration<T> Embed<TProp>(Expression<Func<T, TProp>> property)
		{
			var p = ReflectionUtility.ExtractProperty(property);
			var getter = ReflectionUtility.CreateDelegate<Func<T, TProp>>(p.GetMethod, null);
			HideProperty(p);
			return Embed(p.Name.ToCamelCase(), getter);
		}

	}
}