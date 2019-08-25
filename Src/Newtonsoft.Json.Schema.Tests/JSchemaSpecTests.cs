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
                AddSchema(resolver, "draft3.json", "http://json-schema.org/draft-03/schema");
                AddSchema(resolver, "draft4.json", "http://json-schema.org/draft-04/schema");
                AddSchema(resolver, "draft6.json", "http://json-schema.org/draft-06/schema");
                AddSchema(resolver, "draft7.json", "http://json-schema.org/draft-07/schema");
                AddSchema(resolver, "integer.json", "http://localhost:1234/integer.json");
                AddSchema(resolver, "folder/folderInteger.json", "http://localhost:1234/folder/folderInteger.json");
                AddSchema(resolver, "subSchemas.json", "http://localhost:1234/subSchemas.json");
                AddSchema(resolver, "name.json", "http://localhost:1234/name.json");

                _resolver = resolver;
            }

            return _resolver;
        }

        private static void AddSchema(JSchemaPreloadedResolver resolver, string schemaFileName, string id)
        {
            string baseRemotePath = ResolvePath(Path.Combine("Specs", "remotes"));

            string json = File.ReadAllText(Path.Combine(baseRemotePath, schemaFileName));

            resolver.Add(new Uri(id), json);
        }

        private Uri GetSchemaUri(string version)
        {
            SchemaVersion schemaVersion = (SchemaVersion)Enum.Parse(typeof(SchemaVersion), version);

            return SchemaVersionHelpers.MapSchemaVersion(schemaVersion);
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

            JSchema s = JSchema.Load(schemaSpecTest.Schema.CreateReader(), resolver);
            s.SchemaVersion = GetSchemaUri(schemaSpecTest.Version);

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

            JSchema s = JSchema.Load(schemaSpecTest.Schema.CreateReader(), resolver);
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

                    JArray a = JArray.Parse(testJson);

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
                            if (schemaSpecTest.FileName == "format.json" &&
                                schemaSpecTest.TestDescription == "a valid URL ")
                            {
                                continue;
                            }
#endif

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
    }
}