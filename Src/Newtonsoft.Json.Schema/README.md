# ![Logo](https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json.Schema/master/Doc/icons/logo.jpg) Json.NET Schema

[![NuGet version (Newtonsoft.Json.Schema)](https://img.shields.io/nuget/v/Newtonsoft.Json.Schema.svg?style=flat-square)](https://www.nuget.org/packages/Newtonsoft.Json.Schema/)
[![Build status](https://dev.azure.com/jamesnk/Public/_apis/build/status/JamesNK.Newtonsoft.Json.Schema?branchName=master)](https://dev.azure.com/jamesnk/Public/_build/latest?definitionId=10)

Json.NET Schema is a powerful, complete and easy to use JSON Schema framework for .NET

## Validate JSON

```csharp
JSchema schema = JSchema.Parse(@"{
  'type': 'object',
  'properties': {
    'name': {'type':'string'},
    'roles': {'type': 'array'}
  }
}");

JObject user = JObject.Parse(@"{
  'name': 'Arnie Admin',
  'roles': ['Developer', 'Administrator']
}");

bool valid = user.IsValid(schema);
// true
```

## Generate Schemas

```csharp
JSchemaGenerator generator = new JSchemaGenerator();
JSchema schema = generator.Generate(typeof(Account));
// {
//   "type": "object",
//   "properties": {
//     "email": { "type": "string", "format": "email" }
//   },
//   "required": [ "email" ]
// }

public class Account
{
    [EmailAddress]
    [JsonProperty("email", Required = Required.Always)]
    public string Email;
}
```

## Validate Deserialization

```csharp
JSchema schema = JSchema.Parse(@"{
  'type': 'array',
  'item': {'type':'string'}
}");
JsonTextReader reader = new JsonTextReader(new StringReader(@"[
  'Developer',
  'Administrator'
]"));

JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
validatingReader.Schema = schema;

JsonSerializer serializer = new JsonSerializer();
List<string> roles = serializer.Deserialize<List<string>>(validatingReader);
```

## Links

- [Homepage](http://www.newtonsoft.com/jsonschema)
- [Documentation](http://www.newtonsoft.com/jsonschema/help)
- [NuGet Package](https://www.nuget.org/packages/Newtonsoft.Json.Schema)
- [Release Notes](https://github.com/JamesNK/Newtonsoft.Json.Schema/releases)
- [License](https://github.com/JamesNK/Newtonsoft.Json.Schema/blob/master/LICENSE.md)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/json.net+jsonschema)
