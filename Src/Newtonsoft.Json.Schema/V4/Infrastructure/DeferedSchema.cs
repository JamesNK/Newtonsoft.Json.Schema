using System;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class DeferedSchema
    {
        public readonly Uri ResolvedReference;
        public readonly JSchema4 ReferenceSchema;
        public readonly Action<JSchema4> SetSchema;

        public DeferedSchema(Uri resolvedReference, JSchema4 referenceSchema, Action<JSchema4> setSchema)
        {
            ResolvedReference = resolvedReference;
            ReferenceSchema = referenceSchema;
            SetSchema = setSchema;
        }
    }
}