#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    [DebuggerDisplay("Reference = {ResolvedReference}, Success = {Success}")]
    internal class DeferedSchema
    {
        public readonly Uri ResolvedReference;
        public readonly JSchema ReferenceSchema;
        public readonly List<Action<JSchema>> SetSchemas;

        private bool _success;

        public bool Success
        {
            get { return _success; }
        }

        public DeferedSchema(Uri resolvedReference, JSchema referenceSchema)
        {
            SetSchemas = new List<Action<JSchema>>();
            ResolvedReference = resolvedReference;
            ReferenceSchema = referenceSchema;
        }

        public void AddSchemaSet(Action<JSchema> setSchema)
        {
            SetSchemas.Add(setSchema);
        }

        public void SetResolvedSchema(JSchema schema)
        {
            foreach (Action<JSchema> setSchema in SetSchemas)
            {
                setSchema(schema);
            }

            // successful
            _success = (schema.Reference == null);
        }
    }
}