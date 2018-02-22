#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class SetSchema
    {
        private readonly Action<JSchema> _setAction;
        private readonly JSchema _target;

        public SetSchema(Action<JSchema> setAction, JSchema target)
        {
            _setAction = setAction;
            _target = target;
        }

        public void Execute(JSchema schema)
        {
            if (_target != null)
            {
                _target.State = JSchemaState.Loading;
            }

            try
            {
                _setAction(schema);
            }
            finally
            {
                if (_target != null)
                {
                    _target.State = JSchemaState.Default;
                }
            }
        }
    }

    [DebuggerDisplay("Reference = {ResolvedReference}, Success = {Success}")]
    internal class DeferedSchema
    {
        public readonly Uri OriginalReference;
        public readonly Uri ResolvedReference;
        public readonly JSchema ReferenceSchema;
        public readonly List<SetSchema> SetSchemas;

        private bool _success;
        private JSchema _resolvedSchema;

        public bool Success => _success;

        public JSchema ResolvedSchema => _resolvedSchema;

        public DeferedSchema(Uri resolvedReference, Uri originalReference, JSchema referenceSchema)
        {
            SetSchemas = new List<SetSchema>();
            ResolvedReference = resolvedReference;
            OriginalReference = originalReference;
            ReferenceSchema = referenceSchema;
        }

        public void AddSchemaSet(Action<JSchema> setSchema, JSchema target)
        {
            SetSchemas.Add(new SetSchema(setSchema, target));
        }

        public void SetResolvedSchema(JSchema schema)
        {
            // successful
            if (schema.Reference == null)
            {
                _success = true;
                _resolvedSchema = schema;

                schema.Reference = null;

                foreach (SetSchema setSchema in SetSchemas)
                {
                    setSchema.Execute(schema);
                }
            }
            else
            {
                _success = false;
            }
        }
    }
}