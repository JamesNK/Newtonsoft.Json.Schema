#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.ComponentModel;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public sealed class DefaultValueAttributeTestClass
    {
        [DefaultValue("TestProperty1Value")]
        public string TestProperty1 { get; set; }

        [DefaultValue(21)]
        public int TestField1;
    }
}