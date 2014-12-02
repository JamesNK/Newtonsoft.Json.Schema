#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class CircularReferenceClass
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CircularReferenceClass Child { get; set; }
    }
}