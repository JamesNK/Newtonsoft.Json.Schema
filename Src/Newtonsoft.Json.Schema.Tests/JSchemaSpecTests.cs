#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using TestCaseSource = Xunit.MemberDataAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    public class SchemaSpecTest
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public string TestCaseDescription { get; set; }
        public JToken Schema { get; set; }
        public string TestDescription { get; set; }
        public JToken Data { get; set; }
        public bool IsValid { get; set; }
        public int TestNumber { get; set; }

        public override string ToString()
        {
            return Version + " - " + FileName + " - " + TestCaseDescription + " - " + TestDescription;
        }
    }

    [TestFixture]
    public class JSchemaSpecTests : TestFixtureBase
    {
        private static JSchemaPreloadedResolver _resolver;

        private static JSchemaPreloadedResolver GetResolver()
        {
            if (_resolver == null)
            {
                var resolver = new JSchemaPreloadedResolver();
                // Spec schemas
                AddSchema(resolver, "draft3.json", "http://json-schema.org/draft-03/schema", "drafts");
                AddSchema(resolver, "draft4.json", "http://json-schema.org/draft-04/schema", "drafts");
                AddSchema(resolver, "draft6.json", "http://json-schema.org/draft-06/schema", "drafts");
                AddSchema(resolver, "draft7.json", "http://json-schema.org/draft-07/schema", "drafts");
                AddSchema(resolver, "draft2019-09/draft2019-09.json", "https://json-schema.org/draft/2019-09/schema", "drafts");
                AddSchema(resolver, "draft2019-09/meta/applicator.json", "https://json-schema.org/draft/2019-09/meta/applicator", "drafts");
                AddSchema(resolver, "draft2019-09/meta/content.json", "https://json-schema.org/draft/2019-09/meta/content", "drafts");
                AddSchema(resolver, "draft2019-09/meta/core.json", "https://json-schema.org/draft/2019-09/meta/core", "drafts");
                AddSchema(resolver, "draft2019-09/meta/format.json", "https://json-schema.org/draft/2019-09/meta/format", "drafts");
                AddSchema(resolver, "draft2019-09/meta/hyper-schema.json", "https://json-schema.org/draft/2019-09/meta/hyper-schema", "drafts");
                AddSchema(resolver, "draft2019-09/meta/meta-data.json", "https://json-schema.org/draft/2019-09/meta/meta-data", "drafts");
                AddSchema(resolver, "draft2019-09/meta/validation.json", "https://json-schema.org/draft/2019-09/meta/validation", "drafts");

                // Test resource schemas
                AddSchema(resolver, "integer.json", "http://localhost:1234/integer.json");
                AddSchema(resolver, "baseUriChange/folderInteger.json", "http://localhost:1234/baseUriChange/folderInteger.json");
                AddSchema(resolver, "baseUriChangeFolder/folderInteger.json", "http://localhost:1234/baseUriChangeFolder/folderInteger.json");
                AddSchema(resolver, "baseUriChangeFolderInSubschema/folderInteger.json", "http://localhost:1234/baseUriChangeFolderInSubschema/folderInteger.json");
                AddSchema(resolver, "subSchemas.json", "http://localhost:1234/subSchemas.json");
                AddSchema(resolver, "subSchemas-defs.json", "http://localhost:1234/subSchemas-defs.json");
                AddSchema(resolver, "name.json", "http://localhost:1234/name.json");
                AddSchema(resolver, "name-defs.json", "http://localhost:1234/name-defs.json");

                AddSchema(resolver, "draft2020-12/prefixItems.json", "http://localhost:1234/draft2020-12/prefixItems.json");
                AddSchema(resolver, "draft7/ignore-dependentRequired.json", "http://localhost:1234/draft7/ignore-dependentRequired.json");
                AddSchema(resolver, "draft2019-09/locationIndependentIdentifier.json", "http://localhost:1234/draft2019-09/locationIndependentIdentifier.json");
                AddSchema(resolver, "draft2019-09/baseUriChange/folderInteger.json", "http://localhost:1234/draft2019-09/baseUriChange/folderInteger.json");
                AddSchema(resolver, "draft2019-09/baseUriChangeFolder/folderInteger.json", "http://localhost:1234/draft2019-09/baseUriChangeFolder/folderInteger.json");
                AddSchema(resolver, "draft2019-09/baseUriChangeFolderInSubschema/folderInteger.json", "http://localhost:1234/draft2019-09/baseUriChangeFolderInSubschema/folderInteger.json");
                AddSchema(resolver, "draft2019-09/subSchemas-defs.json", "http://localhost:1234/draft2019-09/subSchemas-defs.json");
                AddSchema(resolver, "different-id-ref-string.json", "http://localhost:1234/different-id-ref-string.json");
                AddSchema(resolver, "urn-ref-string.json", "http://localhost:1234/urn-ref-string.json");
                AddSchema(resolver, "nested-absolute-ref-to-string.json", "http://localhost:1234/nested-absolute-ref-to-string.json");
                AddSchema(resolver, "draft2019-09/integer.json", "http://localhost:1234/draft2019-09/integer.json");
                AddSchema(resolver, "draft2019-09/ref-and-defs.json", "http://localhost:1234/draft2019-09/ref-and-defs.json");
                AddSchema(resolver, "draft2019-09/nested/foo-ref-string.json", "http://localhost:1234/draft2019-09/nested/foo-ref-string.json");
                AddSchema(resolver, "draft2019-09/nested/string.json", "http://localhost:1234/draft2019-09/nested/string.json");
                AddSchema(resolver, "draft2019-09/name-defs.json", "http://localhost:1234/draft2019-09/name-defs.json");
                AddSchema(resolver, "locationIndependentIdentifierDraft4.json", "http://localhost:1234/locationIndependentIdentifierDraft4.json");
                AddSchema(resolver, "locationIndependentIdentifierPre2019.json", "http://localhost:1234/locationIndependentIdentifierPre2019.json");
                AddSchema(resolver, "ref-and-definitions.json", "http://localhost:1234/ref-and-definitions.json");
                AddSchema(resolver, "nested/foo-ref-string.json", "http://localhost:1234/nested/foo-ref-string.json");
                AddSchema(resolver, "nested/string.json", "http://localhost:1234/nested/string.json");
                AddSchema(resolver, "draft2019-09/dependentRequired.json", "http://localhost:1234/draft2019-09/dependentRequired.json");

                _resolver = resolver;
            }

            return _resolver;
        }

        private static void AddSchema(JSchemaPreloadedResolver resolver, string schemaFileName, string id, string subDirectory = null)
        {
            string baseRemotePath = ResolvePath(Path.Combine("Specs", subDirectory ?? "remotes"));

            string json = File.ReadAllText(Path.Combine(baseRemotePath, schemaFileName));

            resolver.Add(new Uri(id), json);
        }

#if DNXCORE50
        [Theory]
#endif
        [TestCaseSource(nameof(GetSpecTestDetails))]
        public void ReadSpecTest(SchemaSpecTest schemaSpecTest)
        {
            Console.WriteLine("Running reader JSON Schema {0} test {1}: {2}", schemaSpecTest.Version, schemaSpecTest.TestNumber, schemaSpecTest);

            IList<string> errorMessages = new List<string>();

            JSchemaPreloadedResolver resolver = GetResolver();

            var schemaToken = schemaSpecTest.Schema.DeepClone();
            SetSchemaVersion(schemaSpecTest, schemaToken);

            JSchema s = JSchema.Load(schemaToken.CreateReader(), new JSchemaReaderSettings
            {
                Resolver = resolver
            });

            JsonReader jsonReader = schemaSpecTest.Data.CreateReader();

            using (JSchemaValidatingReader reader = new JSchemaValidatingReader(jsonReader))
            {
                reader.Schema = s;
                reader.ValidationEventHandler += (sender, args) => errorMessages.Add(args.Message);

                while (reader.Read())
                {
                }
            }

            bool isValid = (errorMessages.Count == 0);

            Assert.AreEqual(schemaSpecTest.IsValid, isValid, schemaSpecTest.TestCaseDescription + " - " + schemaSpecTest.TestDescription + " - errors: " + StringHelpers.Join(", ", errorMessages));
        }

#if DNXCORE50
        [Theory]
#endif
        [TestCaseSource(nameof(GetSpecTestDetails))]
        public void WriteSpecTest(SchemaSpecTest schemaSpecTest)
        {
            Console.WriteLine("Running writer JSON Schema {0} test {1}: {2}", schemaSpecTest.Version, schemaSpecTest.TestNumber, schemaSpecTest);

            IList<string> errorMessages = new List<string>();

            JSchemaPreloadedResolver resolver = GetResolver();

            var schemaToken = schemaSpecTest.Schema.DeepClone();
            SetSchemaVersion(schemaSpecTest, schemaToken);

            JSchema s = JSchema.Load(schemaToken.CreateReader(), resolver);
            s.SchemaVersion = GetSchemaUri(schemaSpecTest.Version);

            JsonReader jsonReader = schemaSpecTest.Data.CreateReader();

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            using (JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer))
            {
                validatingWriter.Schema = s;
                validatingWriter.ValidationEventHandler += (sender, args) => errorMessages.Add(args.Message);

                while (jsonReader.Read())
                {
                    validatingWriter.WriteToken(jsonReader.TokenType, jsonReader.Value);
                }
            }

            bool isValid = (errorMessages.Count == 0);

            Assert.AreEqual(schemaSpecTest.IsValid, isValid, schemaSpecTest.TestCaseDescription + " - " + schemaSpecTest.TestDescription + " - errors: " + StringHelpers.Join(", ", errorMessages));
        }

        private static IList<SchemaSpecTest> _specTests;

        public static IList<object[]> GetSpecTestDetails()
        {
            if (_specTests == null)
            {
                _specTests = new List<SchemaSpecTest>();

                // get test files location relative to the test project dll
                string baseTestPath = ResolvePath(Path.Combine("Specs", "tests")) + @"\";

                string[] testFiles = Directory.GetFiles(baseTestPath, "*.json", SearchOption.AllDirectories);

#if (PORTABLE || NET35)
                testFiles = testFiles.Where(f => !f.EndsWith("bignum.json")).ToArray();
                testFiles = testFiles.Where(f => !f.EndsWith("format.json")).ToArray();
#endif
                testFiles = testFiles.Where(f => !f.EndsWith("non-bmp-regex.json")).ToArray();
                testFiles = testFiles.Where(f => !f.EndsWith("ecmascript-regex.json")).ToArray();
                testFiles = testFiles.Where(f => !f.EndsWith("float-overflow.json")).ToArray();
                testFiles = testFiles.Where(f => !f.EndsWith("vocabulary.json")).ToArray();

                // todo - add support for all formats
                testFiles = testFiles.Where(f => !f.EndsWith("content.json")
                                                 && !f.EndsWith("idn-email.json")
                                                 && !f.EndsWith("idn-hostname.json")
                                                 && !f.EndsWith("iri-reference.json")
                                                 && !f.EndsWith("iri.json")
                                                 && !f.EndsWith("relative-json-pointer.json")).ToArray();

                // read through each of the *.json test files and extract the test details
                foreach (string testFile in testFiles)
                {
                    string relativePath = MakeRelativePath(baseTestPath, testFile);
                    string version = relativePath.Split('\\').First();
                    string testJson = System.IO.File.ReadAllText(testFile);

                    JsonTextReader testJsonReader = new JsonTextReader(new StringReader(testJson));
                    testJsonReader.DateParseHandling = DateParseHandling.None;
                    testJsonReader.FloatParseHandling = testFile.EndsWith("const.json")
                        ? FloatParseHandling.Decimal
                        : FloatParseHandling.Double;

                    JArray a = (JArray)JToken.ReadFrom(testJsonReader);

                    foreach (JObject testCase in a)
                    {
                        foreach (JObject test in testCase["tests"])
                        {
                            SchemaSpecTest schemaSpecTest = new SchemaSpecTest();

                            schemaSpecTest.FileName = Path.GetFileName(testFile);
                            schemaSpecTest.Version = version;
                            schemaSpecTest.TestCaseDescription = (string)testCase["description"];
                            schemaSpecTest.Schema = testCase["schema"];

                            schemaSpecTest.TestDescription = (string)test["description"];
                            schemaSpecTest.Data = test["data"];
                            schemaSpecTest.IsValid = (bool)test["valid"];
                            schemaSpecTest.TestNumber = _specTests.Count(t => t.Version == schemaSpecTest.Version) + 1;

#if NET35
                            // Uri class in .NET Framework 2.0 doesn't like the uri in this test
                            if ((schemaSpecTest.FileName == "format.json" || schemaSpecTest.FileName == "uri.json") &&
                                schemaSpecTest.TestDescription == "a valid URL ")
                            {
                                continue;
                            }
#endif
                            // Allow time without offset for backwards compatibility.
                            if (schemaSpecTest.FileName == "time.json" &&
                                (schemaSpecTest.TestDescription == "no time offset with second fraction" || schemaSpecTest.TestDescription == "no time offset"))
                            {
                                continue;
                            }
                            // Can't test support the future because the future isn't supported yet.
                            if (schemaSpecTest.Version == "Draft2019-09" &&
                                schemaSpecTest.FileName == "cross-draft.json" &&
                                schemaSpecTest.TestCaseDescription == "refs to future drafts are processed as future drafts")
                            {
                                continue;
                            }

                            _specTests.Add(schemaSpecTest);
                        }
                    }
                }
            }

            return _specTests.Select(s => new object[] { s }).ToList();
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new Uri(fromPath, UriKind.RelativeOrAbsolute);
            Uri toUri = new Uri(toPath, UriKind.RelativeOrAbsolute);

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static Uri GetSchemaUri(string version)
        {
            SchemaVersion schemaVersion = (SchemaVersion)Enum.Parse(typeof(SchemaVersion), version.Replace('-', '_'));

            return SchemaVersionHelpers.MapSchemaVersion(schemaVersion);
        }

        private static void SetSchemaVersion(SchemaSpecTest schemaSpecTest, JToken schemaToken)
        {
            if (schemaToken.Type == JTokenType.Object)
            {
                var o = (JObject)schemaToken;
                o.Remove("$schema");
                o.AddFirst(new JProperty("$schema", GetSchemaUri(schemaSpecTest.Version)));
            }
        }
    }
}