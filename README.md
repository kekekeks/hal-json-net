hal-json-net
============

HAL JSON support for Json.Net


Model:

```csharp

public class Model
{
	public uint Id { get; set; }
	public string Name { get; set; }
	public List<int> ItemIds { get; set; }
}
```


Configuration:

```csharp
var config = HalJsonConfiguration("http://example.com/api"); //You can use null instead of example.com, it won't touch your links in that case
config.Configure<Model>()
				.Embed(p => p.ItemIds)
				.Link("self", x => "/model/" + x.Id)
				.Link("all", "/model");
				
var serializer = new JsonSerializer{ContractResolver = new JsonNetHalJsonContactResolver(config)};
//Use serializer here
```

Results:

```js
{
  "_links": {
    "all": {
      "href": "http://example.com/api/model",
      "templated": "false"
    },
    "self": {
      "href": "http://example.com/api/model/42",
      "templated": "false"
    }
  },
  "_embedded": {
    "itemIds": [
      1,
      2,
      3
    ]
  },
  "id": 42,
  "name": "One"
}


```

You may as well use IHaveHalJsonLinks and IHaveHalJsonEmbedded:

```csharp

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

```

Or attributes:

```csharp

[HalJsonLink("something", "123")]
public class AttributeBasedModel
{
    [HalJsonLink("self", "/mdl/{0}")]
    public int Id { get; set; }

    [HalJsonEmbedded("ids")]
    public List<int> Ids { get; set; }
}

```

Or even mix them together.


# Differencies from other JSON.NET libraries

###### https://github.com/MLaritz/HalJsonConverter


- You don't have to hardcode HAL information inside the model (it's impossible to configure base url in MLaritz's implementation)
- My implementation doesn't duplicate Json.Net serialization infrastructure, just changes it's configuration, you won't loss your serialization settings configured by Json.Net built-in attribute system (i.e. JsonProperty, JsonIgnore, before/after serialization callbacks, etc)

