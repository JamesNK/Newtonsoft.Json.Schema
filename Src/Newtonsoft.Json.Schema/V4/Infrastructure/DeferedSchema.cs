using System;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class DeferedSchema
    {
        public readonly Uri Reference;
        public readonly Action<JSchema4> SetSchema;

        public DeferedSchema(Uri reference, Action<JSchema4> setSchema)
        {
            Reference = reference;
            SetSchema = setSchema;
        }
    }
}