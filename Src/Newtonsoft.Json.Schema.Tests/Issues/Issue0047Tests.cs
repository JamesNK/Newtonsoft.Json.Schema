#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Schema.Generation;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0047Tests : TestFixtureBase
    {
        [DataContract]
        public class HttpBrokeredMessage
        {
            [DataMember]
            public string Body { get; set; }

            [DataMember]
            public Dictionary<string, object> BrokerProperties { get; set; }

            [DataMember]
            public bool IsBodyBase64 { get; set; }

            [DataMember]
            public Dictionary<string, object> UserProperties { get; set; }
        }

        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema s = generator.Generate(typeof(HttpBrokeredMessage));

            Assert.AreEqual(null, s.Properties[nameof(HttpBrokeredMessage.BrokerProperties)].AdditionalProperties.Type);
            Assert.AreEqual(null, s.Properties[nameof(HttpBrokeredMessage.UserProperties)].AdditionalProperties.Type);
        }

        [Test]
        public void Test_DisallowNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.DisallowNull
            };

            JSchema s = generator.Generate(typeof(HttpBrokeredMessage));

            JSchemaType allExcludingNull = JSchemaType.Array | JSchemaType.Object | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Number | JSchemaType.String;

            Assert.AreEqual(allExcludingNull, s.Properties[nameof(HttpBrokeredMessage.BrokerProperties)].AdditionalProperties.Type);
            Assert.AreEqual(allExcludingNull, s.Properties[nameof(HttpBrokeredMessage.UserProperties)].AdditionalProperties.Type);
        }
    }
}