#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class SchemaVersionHelpers
    {
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

        private static readonly SchemaVersionMap[] VersionMap =
        {
            new SchemaVersionMap(SchemaVersion.Draft3, Constants.SchemaVersions.Draft3, "schema-draft-v3.json"),
            new SchemaVersionMap(SchemaVersion.Draft4, Constants.SchemaVersions.Draft4, "schema-draft-v4.json"),
            new SchemaVersionMap(SchemaVersion.Draft6, Constants.SchemaVersions.Draft6, "schema-draft-v6.json"),
            new SchemaVersionMap(SchemaVersion.Draft7, Constants.SchemaVersions.Draft7, "schema-draft-v7.json"),
            new SchemaVersionMap(SchemaVersion.Draft2019_09, Constants.SchemaVersions.Draft2019_09, "draft2019-09/schema.json"),
        };

        private static readonly ThreadSafeStore<string, JSchema> SpecSchemaCache = new ThreadSafeStore<string, JSchema>(LoadResourceSchema);

        private static JSchema LoadResourceSchema(string name)
        {
            using (Stream schemaData = typeof(JSchemaReader).Assembly().GetManifestResourceStream("Newtonsoft.Json.Schema.Resources." + name))
            using (StreamReader sr = new StreamReader(schemaData))
            {
                return JSchema.Load(new JsonTextReader(sr));
            }
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
