#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(ASPNETCORE50 || NETFX_CORE)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.V4;

namespace Newtonsoft.Json.Schema.Tests
{
    public class JsonSchemaSpecTest
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public string TestCaseDescription { get; set; }
        public JObject Schema { get; set; }
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
    public class JsonSchemaSpecTests : TestFixtureBase
    {
        private static JSchemaPreloadedResolver _resolver;

        private static JSchemaPreloadedResolver GetResolver()
        {
            if (_resolver == null)
            {
                var resolver = new JSchemaPreloadedResolver();
                AddSchema(resolver, "draft3.json", "http://json-schema.org/draft-03/schema");
                AddSchema(resolver, "draft4.json", "http://json-schema.org/draft-04/schema");
                AddSchema(resolver, "integer.json", "http://localhost:1234/integer.json");
                AddSchema(resolver, "folder/folderInteger.json", "http://localhost:1234/folder/folderInteger.json");
                AddSchema(resolver, "subSchemas.json", "http://localhost:1234/subSchemas.json");
                
                _resolver = resolver;
            }

            return _resolver;
        }

        private static void AddSchema(JSchemaPreloadedResolver resolver, string schemaFileName, string id)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string baseRemotePath = Path.Combine(baseDirectory, "Specs", "remotes");

            string json = File.ReadAllText(Path.Combine(baseRemotePath, schemaFileName));

            JSchema4 schema = JSchema4.Parse(json);

            resolver.Add(new Uri(id), schema);
        }

        [TestCaseSourceAttribute("GetSpecTestDetails")]
        public void SpecTest(JsonSchemaSpecTest jsonSchemaSpecTest)
        {
            Console.WriteLine("Running JSON Schema {0} test {1}: {2}", jsonSchemaSpecTest.Version, jsonSchemaSpecTest.TestNumber, jsonSchemaSpecTest);

            JSchemaPreloadedResolver resolver = GetResolver();

            JSchema4 s = JSchema4.Read(jsonSchemaSpecTest.Schema.CreateReader(), resolver);

            IList<string> errorMessages;
            bool v = jsonSchemaSpecTest.Data.IsValid(s, out errorMessages);
            errorMessages = errorMessages ?? new List<string>();

            Assert.AreEqual(jsonSchemaSpecTest.IsValid, v, jsonSchemaSpecTest.TestCaseDescription + " - " + jsonSchemaSpecTest.TestDescription + " - errors: " + string.Join(", ", errorMessages));
        }

        public IList<JsonSchemaSpecTest> GetSpecTestDetails()
        {
            IList<JsonSchemaSpecTest> specTests = new List<JsonSchemaSpecTest>();

            // get test files location relative to the test project dll
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string baseTestPath = Path.Combine(baseDirectory, "Specs", "tests");

            string[] testFiles = Directory.GetFiles(baseTestPath, "*.json", SearchOption.AllDirectories);

            // read through each of the *.json test files and extract the test details
            foreach (string testFile in testFiles)
            {
                string testJson = System.IO.File.ReadAllText(testFile);

                JArray a = JArray.Parse(testJson);

                foreach (JObject testCase in a)
                {
                    foreach (JObject test in testCase["tests"])
                    {
                        JsonSchemaSpecTest jsonSchemaSpecTest = new JsonSchemaSpecTest();

                        jsonSchemaSpecTest.FileName = Path.GetFileName(testFile);
                        jsonSchemaSpecTest.Version = Directory.GetParent(testFile).Name;
                        jsonSchemaSpecTest.TestCaseDescription = (string)testCase["description"];
                        jsonSchemaSpecTest.Schema = (JObject)testCase["schema"];

                        jsonSchemaSpecTest.TestDescription = (string)test["description"];
                        jsonSchemaSpecTest.Data = test["data"];
                        jsonSchemaSpecTest.IsValid = (bool)test["valid"];
                        jsonSchemaSpecTest.TestNumber = specTests.Count(t => t.Version == jsonSchemaSpecTest.Version) + 1;

                        specTests.Add(jsonSchemaSpecTest);
                    }
                }
            }

            return specTests;
        }
    }
}
#endif