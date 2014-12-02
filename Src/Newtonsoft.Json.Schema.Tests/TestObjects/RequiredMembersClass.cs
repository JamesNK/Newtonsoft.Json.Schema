#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class RequiredMembersClass
    {
        [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty]
        public string MiddleName { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string LastName { get; set; }

        [JsonProperty(Required = Required.Default)]
        public DateTime BirthDate { get; set; }
    }
}