using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace HalJsonNet.Serialization
{
	public class JsonNetHalJsonContactResolver : DefaultContractResolver
	{
		private readonly HalJsonConfiguration _configuration;
		private readonly bool _camelCase;

		public JsonNetHalJsonContactResolver(HalJsonConfiguration configuration, bool camelCase = true) : base(false)
		{
			_configuration = configuration;
			_camelCase = camelCase;
		}

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			HalJsonTypeConfiguration config;
			if (!_configuration.TryGetTypeConfiguration(type, out config))
				return base.CreateProperties(type, memberSerialization);
				
			var ignored = config.HiddenProperties;

			var lst = new List<JsonProperty>();
			foreach (var member in GetSerializableMembers(type))
			{
				var prop = CreateProperty(member, memberSerialization);
				if (ignored.Contains(member))
					prop.Readable = false;
				lst.Add(prop);
			}
			lst = lst.OrderBy(p => p.Order ?? -1).ToList();
			if (config.Embedded.Count != 0)
			{
				var property = new JsonProperty
				{
					PropertyType = typeof (IDictionary<string, object>),
					DeclaringType = type,
					Writable = false,
					Readable = true,
					ValueProvider = new EmbeddedValueProvider(config.Embedded),
					PropertyName = "_embedded"
				};
				lst.Insert(0, property);
			}
			if (config.Links.Count != 0)
			{
				lst.Insert(0, new JsonProperty
				{
					PropertyType = typeof (IDictionary<string, object>),
					DeclaringType = type,
					Writable = false,
					Readable = true,
					ValueProvider = new LinksValueProvider(config.Links),
					PropertyName = "_links"
				});

			}

			return lst;
		}


		private class LinksValueProvider : IValueProvider
		{
			private readonly IReadOnlyDictionary<string, Link> _links;

			public LinksValueProvider(IReadOnlyDictionary<string, Link> links)
			{
				_links = links;
			}

			public void SetValue(object target, object value)
			{
				throw new NotSupportedException();
			}


			public object GetValue(object target)
			{
				return _links.ToDictionary(x => x.Key, x => (object) new JObject
				{
					{"href", x.Value.GetHref(target)},
					{"templated", x.Value.Templated}
				});
			}
		}

		class EmbeddedValueProvider : IValueProvider
		{
			private readonly IReadOnlyDictionary<string, Embedded> _embed;

			public EmbeddedValueProvider(IReadOnlyDictionary<string, Embedded> embed)
			{
				_embed = embed;
			}

			public void SetValue(object target, object value)
			{
				throw new NotSupportedException();
			}

			public object GetValue(object target)
			{
				return _embed.ToDictionary(x => x.Key, x => x.Value.Getter(target));
			}
		}

		protected override string ResolvePropertyName(string propertyName)
		{
			if (_camelCase)
				propertyName = propertyName.ToCamelCase();
			return base.ResolvePropertyName(propertyName);
		}
	}
}
