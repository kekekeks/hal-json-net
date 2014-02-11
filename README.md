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
var config = HalJsonConfiguration();
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
      "href": "/model",
      "templated": "false"
    },
    "self": {
      "href": "/model/42",
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
