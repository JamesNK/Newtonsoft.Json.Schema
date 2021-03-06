#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    [JsonObject(Id = "Person", Title = "Title!", Description = "JsonObjectAttribute description!", MemberSerialization = MemberSerialization.OptIn)]
    [Description("DescriptionAttribute description!")]
    public class Person
    {
        // "John Smith"
        [JsonProperty]
        public string Name { get; set; }

        // "2000-12-15T22:11:03"
        [JsonProperty]
        //[JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime BirthDate { get; set; }

        // new Date(976918263055)
        [JsonProperty]
        //[JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime LastModified { get; set; }

        // not serialized
        public string Department { get; set; }
    }

    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime BirthDate { get; set; }
    }

    public class Employee : IPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public string Department { get; set; }
        public string JobTitle { get; set; }
    }

    public class Manager : Employee
    {
        public IList<Employee> Employees { get; set; }
    }
}