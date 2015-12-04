#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class PerformanceTests : TestFixtureBase
    {
        private const int ValidationCount = 1000;

        [Test]
        public void IsValidPerformance()
        {
            JArray a = JArray.Parse(Json);
            a.IsValid(Schema);

            using (var tester = new PerformanceTester("IsValid"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    a.IsValid(Schema);
                }
            }
        }

        [Test]
        public void IsValidPerformance_Failure()
        {
            JArray a = JArray.Parse(JsonFailure);
            a.IsValid(Schema);

            using (var tester = new PerformanceTester("IsValid_Failure"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    a.IsValid(Schema);
                }
            }
        }

        [Test]
        public void IsValid_SchemaSpec()
        {
            string schemaJson = TestHelpers.OpenFileText(@"resources\schemas\schema-draft-v4.json");
            JSchema s = JSchema.Parse(schemaJson);
            JObject o = JObject.Parse(schemaJson);
            o.IsValid(s);

            using (var tester = new PerformanceTester("IsValid_SchemaSpec"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    o.IsValid(s);
                }
            }
        }

        [Test]
        public void ReaderPerformance()
        {
            ReaderValidation();

            using (var tester = new PerformanceTester("Reader"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    ReaderValidation();
                }
            }
        }

        private void ReaderValidation()
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(Json));
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = Schema;

            while (validatingReader.Read())
            {
            }
        }

        private readonly JSchema Schema = JSchema.Parse(@"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""title"": ""Product set"",
    ""type"": ""array"",
    ""items"": {
        ""title"": ""Product"",
        ""type"": ""object"",
        ""properties"": {
            ""id"": {
                ""description"": ""The unique identifier for a product"",
                ""type"": ""number""
            },
            ""name"": {
                ""type"": ""string""
            },
            ""price"": {
                ""type"": ""number"",
                ""minimum"": 0,
                ""exclusiveMinimum"": true
            },
            ""tags"": {
                ""type"": ""array"",
                ""items"": {
                    ""type"": ""string""
                },
                ""minItems"": 1,
                ""uniqueItems"": true
            },
            ""dimensions"": {
                ""type"": ""object"",
                ""properties"": {
                    ""length"": {""type"": ""number""},
                    ""width"": {""type"": ""number""},
                    ""height"": {""type"": ""number""}
                },
                ""required"": [""length"", ""width"", ""height""]
            },
            ""warehouseLocation"": {
                ""description"": ""A geographical coordinate"",
                ""type"": ""object"",
                ""properties"": {
                    ""latitude"": { ""type"": ""number"" },
                    ""longitude"": { ""type"": ""number"" }
                }
            }
        },
        ""required"": [""id"", ""name"", ""price""]
    }
}");

        private const string Json = @"[
    {
        ""id"": 2,
        ""name"": ""An ice sculpture"",
        ""price"": 12.50,
        ""tags"": [""cold"", ""ice""],
        ""dimensions"": {
            ""length"": 7.0,
            ""width"": 12.0,
            ""height"": 9.5
        },
        ""warehouseLocation"": {
            ""latitude"": -78.75,
            ""longitude"": 20.4
        }
    },
    {
        ""id"": 3,
        ""name"": ""A blue mouse"",
        ""price"": 25.50,
        ""dimensions"": {
            ""length"": 3.1,
            ""width"": 1.0,
            ""height"": 1.0
        },
        ""warehouseLocation"": {
            ""latitude"": 54.4,
            ""longitude"": -32.7
        }
    }
]";

        private const string JsonFailure = @"[
    {
        ""id"": ""2"",
        ""name"": ""An ice sculpture"",
        ""price"": 12.50,
        ""tags"": [""cold"", ""ice""],
        ""dimensions"": {
            ""length"": 7.0,
            ""width"": 12.0,
            ""height"": 9.5
        },
        ""warehouseLocation"": {
            ""latitude"": -78.75,
            ""longitude"": 20.4
        }
    },
    {
        ""id"": 3,
        ""name"": ""A blue mouse"",
        ""dimensions"": {
            ""height"": 1.0
        },
        ""warehouseLocation"": {
            ""latitude"": 54.4,
            ""longitude"": -32.7
        }
    }
]";
    }

    public class PerformanceTester : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Action<TimeSpan> _callback;

        public PerformanceTester(string description)
            : this(ts => Console.WriteLine(description + ": " + ts.TotalSeconds))
        {
        }

        public PerformanceTester(Action<TimeSpan> callback)
        {
            _callback = callback;
            _stopwatch.Start();
        }

        public static PerformanceTester Start(Action<TimeSpan> callback)
        {
            return new PerformanceTester(callback);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (_callback != null)
            {
                _callback(Result);
            }
        }

        public TimeSpan Result
        {
            get { return _stopwatch.Elapsed; }
        }
    }
}