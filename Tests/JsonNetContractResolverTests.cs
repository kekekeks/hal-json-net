using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HalJsonNet;
using HalJsonNet.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tests
{
    public class JsonNetContractResolverTests
	{
		public class Model
		{
			public uint Id { get; set; }
			public string Name { get; set; }
			public List<int> Ids { get; set; }
		}

		HalJsonConfiguration Configure(string baseUrl = null)
	    {
			var config = new HalJsonConfiguration (baseUrl);
		    config.Configure<Model>()
		        .Embed(p => p.Ids)
		        .Link("self", x => "/model/" + x.Id)
		        .Link("all", "/model")
		        .Link("external", "http://example.com/example");
			
		    return config;
	    }

		[Fact]
	    public void ShouldEmbedWhatShouldBeEmbedded()
	    {
		    var model = new Model {Name = "One", Ids = new List<int> {1, 2, 3}};
		    var res = SerializeAsJson(Configure(), model);
			Assert.Equal(3, ((JArray) res["_embedded"]["ids"])[2].Value<int>());
	    }

		[Fact]
	    public void ShouldAddLinks()
	    {
		    var model = new Model {Id = 42, Name = "One", Ids = new List<int> {1, 2, 3}};
			var res = SerializeAsJson (Configure (), model);
		    Assert.Equal("/model/42", res["_links"]["self"]["href"].Value<string>());
			Assert.Equal ("/model", res["_links"]["self"]["all"].Value<string> ());
	    }

        [Fact]
        public void ShouldRespectBaseUrl()
        {
            var res = SerializeAsJson(Configure("/prefix"), new Model());
            Assert.Equal("http://example.com/example", res["_links"]["external"]["href"]);
            Assert.Equal ("/prefix/model", res["_links"]["all"]["href"]);
        }


        public class AdvancedModel: IHaveHalJsonLinks, IHaveHalJsonEmbedded
        {
            public IDictionary<string, Link> GetLinks()
            {
                return new Dictionary<string, Link> {{"self", "/something"}};
            }

            public IDictionary<string, Embedded> GetEmbedded()
            {
                return new Dictionary<string, Embedded> {{"ids", new Embedded(new[] {1, 2, 3})}};
            }
        }

        [Fact]
        public void ShouldRespectModelInterfaces()
        {
            var model = new AdvancedModel();
            var res = SerializeAsJson(Configure(), model);
            Assert.Equal("/something", res["_links"]["self"]["href"]);
            Assert.Equal(3, ((JArray) res["_embedded"]["ids"])[2].Value<int>());
        }


        string Serialize (HalJsonConfiguration config, object obj)
		{
			var tw = new StringWriter ();
			new JsonSerializer { ContractResolver = new JsonNetHalJsonContactResolver (config) }.Serialize (tw, obj);
			return tw.ToString ();
		}

		JObject SerializeAsJson (HalJsonConfiguration config, object obj)
		{
			return JObject.Parse (Serialize (config, obj));
		}
    }
}
