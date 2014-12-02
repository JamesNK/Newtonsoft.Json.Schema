#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    [JsonObject(Id = "MyExplicitId")]
    public class CircularReferenceWithIdClass
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string Name { get; set; }

        public CircularReferenceWithIdClass Child { get; set; }
    }
}