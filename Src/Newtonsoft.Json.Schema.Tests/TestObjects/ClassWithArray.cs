#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class ClassWithArray
    {
        private readonly IList<long> bar;
        private string foo;

        public ClassWithArray()
        {
            bar = new List<long>() { int.MaxValue };
        }

        [JsonProperty("foo")]
        public string Foo
        {
            get => foo;
            set => foo = value;
        }

        [JsonProperty(PropertyName = "bar")]
        public IList<long> Bar => bar;
    }
}