#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class DeferedSchema
    {
        public readonly Uri ResolvedReference;
        public readonly JSchema ReferenceSchema;
        public readonly Action<JSchema> SetSchema;

        public DeferedSchema(Uri resolvedReference, JSchema referenceSchema, Action<JSchema> setSchema)
        {
            ResolvedReference = resolvedReference;
            ReferenceSchema = referenceSchema;
            SetSchema = setSchema;
        }
    }
}