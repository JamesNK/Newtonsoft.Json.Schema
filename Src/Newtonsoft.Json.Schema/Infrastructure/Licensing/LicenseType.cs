#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Converters;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum LicenseType
    {
        Test,
        JsonSchemaIndie,
        JsonSchemaBusiness
    }
}