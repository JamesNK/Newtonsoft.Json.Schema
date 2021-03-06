#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Linq;
using System;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class PersonRaw
    {
        private Guid _internalId;
        private string _firstName;
        private string _lastName;
        private JRaw _rawContent;

        [JsonIgnore]
        public Guid InternalId
        {
            get => _internalId;
            set => _internalId = value;
        }

        [JsonProperty("first_name")]
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value;
        }

        public JRaw RawContent
        {
            get => _rawContent;
            set => _rawContent = value;
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get => _lastName;
            set => _lastName = value;
        }
    }
}