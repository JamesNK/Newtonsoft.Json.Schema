#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class ClassWithArray
    {
        private readonly IList<long> bar;
        private string foo;

        public ClassWithArray()
        {
            bar = new List<Int64>() { int.MaxValue };
        }

        [JsonProperty("foo")]
        public string Foo
        {
            get { return foo; }
            set { foo = value; }
        }

        [JsonProperty(PropertyName = "bar")]
        public IList<long> Bar
        {
            get { return bar; }
        }
    }
}