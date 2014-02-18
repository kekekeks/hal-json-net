using System;
using System.Collections.Generic;
using System.Linq;
using HalJsonNet.Configuration;
using HalJsonNet.Configuration.Interfaces;
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
		    {
                if(!(typeof(IHaveHalJsonLinks).IsAssignableFrom(type) || typeof(IHaveHalJsonEmbedded).IsAssignableFrom(type)))
		            return base.CreateProperties(type, memberSerialization);
		        config = new HalJsonTypeConfiguration();
		    }

			var lst = new List<JsonProperty>();
			foreach (var member in GetSerializableMembers(type))
			{
				var prop = CreateProperty(member, memberSerialization);
				if (config.HiddenProperties.Contains(member))
					prop.Readable = false;
				lst.Add(prop);
			}
			lst = lst.OrderBy(p => p.Order ?? -1).ToList();


            
			if (config.Embedded.Count != 0 || typeof(IHaveHalJsonEmbedded).IsAssignableFrom(type))
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
			if (config.Links.Count != 0 || typeof(IHaveHalJsonLinks).IsAssignableFrom(type))
			{
				lst.Insert(0, new JsonProperty
				{
					PropertyType = typeof (IDictionary<string, object>),
					DeclaringType = type,
					Writable = false,
					Readable = true,
					ValueProvider = new LinksValueProvider(config.Links, _configuration),
					PropertyName = "_links"
				});

			}

			return lst;
		}


		private class LinksValueProvider : IValueProvider
		{
			private readonly IReadOnlyDictionary<string, Link> _links;
		    private readonly HalJsonConfiguration _configuration;

		    public LinksValueProvider(IReadOnlyDictionary<string, Link> links, HalJsonConfiguration configuration)
			{
			    _links = links;
			    _configuration = configuration;
			}

		    public void SetValue(object target, object value)
			{
				throw new NotSupportedException();
			}

		    string GetHRef(string link)
		    {
		        if (_configuration.UrlBase == null)
		            return link;
		        var relative = link.IndexOf("://", 0, Math.Min(8, link.Length), StringComparison.Ordinal) == -1;
		        if (!relative)
		            return link;
		        if (link.StartsWith("/"))
		            return _configuration.UrlBase + link;
		        return _configuration.UrlBase + "/" + link;
		    }

			public object GetValue(object target)
			{
			    var links = _links.AsEnumerable();
			    var haveLinks = target as IHaveHalJsonLinks;
			    if (haveLinks != null)
			        links = links.Concat(haveLinks.GetLinks());

				return links.ToDictionary(x => x.Key, x => (object) new JObject
				{
					{"href", GetHRef(x.Value.GetHref(target))},
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
			    var embed = _embed.AsEnumerable();
			    var haveEmbed = target as IHaveHalJsonEmbedded;
			    if (haveEmbed != null)
			        embed = embed.Concat(haveEmbed.GetEmbedded());
				return embed.ToDictionary(x => x.Key, x => x.Value.Getter(target));
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
