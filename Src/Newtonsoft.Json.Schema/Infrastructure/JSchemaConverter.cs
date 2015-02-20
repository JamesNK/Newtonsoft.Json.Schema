#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JSchema);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JSchema.Load(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JSchema schema = (JSchema)value;

            schema.WriteTo(writer);
        }
    }
}