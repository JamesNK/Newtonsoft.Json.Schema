#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class SchemaVersionHelpers
    {
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

        public static SchemaVersion? MapSchemaUri(Uri schemaVersionUri)
        {
            if (schemaVersionUri == Constants.SchemaVersions.Draft3)
            {
                return SchemaVersion.Draft3;
            }
            if (schemaVersionUri == Constants.SchemaVersions.Draft4)
            {
                return SchemaVersion.Draft4;
            }
            if (schemaVersionUri == Constants.SchemaVersions.Draft6)
            {
                return SchemaVersion.Draft6;
            }

            return null;
        }

        public static Uri MapSchemaVersion(SchemaVersion? schemaVersion)
        {
            switch (schemaVersion)
            {
                case SchemaVersion.Draft3:
                    return Constants.SchemaVersions.Draft3;
                case SchemaVersion.Draft4:
                    return Constants.SchemaVersions.Draft4;
                case SchemaVersion.Draft6:
                    return Constants.SchemaVersions.Draft6;
                default:
                    return null;
            }
        }
    }
}
