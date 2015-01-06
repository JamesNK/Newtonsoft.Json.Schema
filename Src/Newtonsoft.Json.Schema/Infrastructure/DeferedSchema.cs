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

        private readonly Action<JSchema> _setSchema;
        private bool _success;

        public bool Success
        {
            get { return _success; }
        }

        public DeferedSchema(Uri resolvedReference, JSchema referenceSchema, Action<JSchema> setSchema)
        {
            ResolvedReference = resolvedReference;
            ReferenceSchema = referenceSchema;
            _setSchema = setSchema;
        }

        public void SetResolvedSchema(JSchema schema)
        {
            _setSchema(schema);

            // successfully
            _success = (schema.Reference == null);
        }
    }
}