#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.ComponentModel;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class JsonPropertyWithHandlingValues
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("Default!")]
        public string DefaultValueHandlingIgnoreProperty { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("Default!")]
        public string DefaultValueHandlingIncludeProperty { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("Default!")]
        public string DefaultValueHandlingPopulateProperty { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue("Default!")]
        public string DefaultValueHandlingIgnoreAndPopulateProperty { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NullValueHandlingIgnoreProperty { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string NullValueHandlingIncludeProperty { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Error)]
        public JsonPropertyWithHandlingValues ReferenceLoopHandlingErrorProperty { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public JsonPropertyWithHandlingValues ReferenceLoopHandlingIgnoreProperty { get; set; }

        [JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
        public JsonPropertyWithHandlingValues ReferenceLoopHandlingSerializeProperty { get; set; }
    }
}