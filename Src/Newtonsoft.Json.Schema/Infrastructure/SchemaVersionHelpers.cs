#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class SchemaVersionHelpers
    {
        private static readonly object _lock = new object();
        private static JSchemaPreloadedResolver? SchemaVersionResolver;

        private static readonly SchemaVersionMap[] VersionMap =
        {
            new SchemaVersionMap(SchemaVersion.Draft3, Constants.SchemaVersions.Draft3, "schema-draft-v3.json"),
            new SchemaVersionMap(SchemaVersion.Draft4, Constants.SchemaVersions.Draft4, "schema-draft-v4.json"),
            new SchemaVersionMap(SchemaVersion.Draft6, Constants.SchemaVersions.Draft6, "schema-draft-v6.json"),
            new SchemaVersionMap(SchemaVersion.Draft7, Constants.SchemaVersions.Draft7, "schema-draft-v7.json"),
            new SchemaVersionMap(SchemaVersion.Draft2019_09, Constants.SchemaVersions.Draft2019_09, "draft2019_09.schema.json"),
            new SchemaVersionMap(SchemaVersion.Draft2020_12, Constants.SchemaVersions.Draft2020_12, "draft2020_12.schema.json"),
        };

        private static readonly ThreadSafeStore<string, JSchema> SpecSchemaCache = new ThreadSafeStore<string, JSchema>(LoadResourceSchema);

        private class SchemaVersionMap
        {
            public readonly SchemaVersion Version;
            public readonly Uri VersionUri;
            public readonly string ResourceName;

            public SchemaVersionMap(SchemaVersion version, Uri versionUri, string resourceName)
            {
                Version = version;
                VersionUri = versionUri;
                ResourceName = resourceName;
            }
        }

        private static JSchemaPreloadedResolver GetSchemaResolver()
        {
            if (SchemaVersionResolver == null)
            {
                lock (_lock)
                {
                    if (SchemaVersionResolver == null)
                    {
                        JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/applicator"), "draft2019_09.meta.applicator.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/content"), "draft2019_09.meta.content.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/core"), "draft2019_09.meta.core.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/format"), "draft2019_09.meta.format.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/hyper-schema"), "draft2019_09.meta.hyper-schema.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/meta-data"), "draft2019_09.meta.meta-data.json");
                        AddPreloadedSchema(resolver, new Uri("https://json-schema.org/draft/2019-09/meta/validation"), "draft2019_09.meta.validation.json");

                        SchemaVersionResolver = resolver;
                    }
                }
            }

            return SchemaVersionResolver;
        }

        private static void AddPreloadedSchema(JSchemaPreloadedResolver resolver, Uri uri, string resourceName)
        {
            Stream schemaData = GetEmbeddedResourceStream(resourceName);
            MemoryStream ms = new MemoryStream();
            schemaData.CopyTo(ms);

            resolver.Add(uri, ms.ToArray());
        }

        private static JSchema LoadResourceSchema(string name)
        {
            Stream schemaData = GetEmbeddedResourceStream(name);

            using StreamReader sr = new StreamReader(schemaData);
            return JSchema.Load(new JsonTextReader(sr), GetSchemaResolver());
        }

        private static Stream GetEmbeddedResourceStream(string name)
        {
            string resourceName = "Newtonsoft.Json.Schema.Resources." + name;
            Stream schemaData = typeof(JSchemaReader).Assembly().GetManifestResourceStream(resourceName);
            if (schemaData == null)
            {
                throw new InvalidOperationException("Can't find embedded resource '{0}'.".FormatWith(CultureInfo.InvariantCulture, resourceName));
            }

            return schemaData;
        }

        public static bool EnsureVersion(SchemaVersion currentVersion, SchemaVersion minimum, SchemaVersion? maximum = null)
        {
            if (currentVersion == SchemaVersion.Unset)
            {
                return true;
            }

            if (currentVersion >= minimum)
            {
                if (currentVersion <= maximum || maximum == null)
                {
                    return true;
                }
            }

            return false;
        }

        public static SchemaVersion MapSchemaUri(Uri? schemaVersionUri)
        {
            for (int i = 0; i < VersionMap.Length; i++)
            {
                if (VersionMap[i].VersionUri == schemaVersionUri)
                {
                    return VersionMap[i].Version;
                }
            }

            return SchemaVersion.Unset;
        }

        public static JSchema? GetSchema(SchemaVersion version)
        {
            for (int i = 0; i < VersionMap.Length; i++)
            {
                if (VersionMap[i].Version == version)
                {
                    return SpecSchemaCache.Get(VersionMap[i].ResourceName);
                }
            }

            return null;
        }

        public static Uri? MapSchemaVersion(SchemaVersion? schemaVersion)
        {
            for (int i = 0; i < VersionMap.Length; i++)
            {
                if (VersionMap[i].Version == schemaVersion)
                {
                    return VersionMap[i].VersionUri;
                }
            }

            return null;
        }
    }
}
