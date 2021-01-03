#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class SetSchema
    {
        private readonly Action<JSchema> _setAction;
        private readonly JSchema? _target;

        public SetSchema(Action<JSchema> setAction, JSchema? target)
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

    [DebuggerDisplay("Reference = {ResolvedReference}, IsRecursive = {IsRecursiveReference}, Success = {Success}")]
    internal class DeferedSchema : IIdentiferScope
    {
        public readonly Uri OriginalReference;
        public readonly Uri ResolvedReference;
        public readonly JSchema ReferenceSchema;
        private readonly bool _supportsRef;
        public readonly List<SetSchema> SetSchemas;
        public readonly bool IsRecursiveReference;

        private bool _success;
        private JSchema? _resolvedSchema;

        [MemberNotNullWhen(true, nameof(ResolvedSchema))]
        public bool Success => _success;

        public JSchema? ResolvedSchema => _resolvedSchema;

        public Uri? ScopeId { get; }

        public DeferedSchema(Uri resolvedReference, Uri originalReference, Uri? scopeId, bool isRecursiveReference, JSchema referenceSchema, bool supportsRef)
        {
            SetSchemas = new List<SetSchema>();
            ResolvedReference = resolvedReference;
            OriginalReference = originalReference;
            ScopeId = scopeId;
            IsRecursiveReference = isRecursiveReference;
            ReferenceSchema = referenceSchema;
            _supportsRef = supportsRef;
        }

        public void AddSchemaSet(Action<JSchema> setSchema, JSchema? target)
        {
            SetSchemas.Add(new SetSchema(setSchema, target));
        }

        public void SetResolvedSchema(JSchema schema)
        {
            // Successful if there is no reference, or we can set $ref
            if (!schema.HasReference || (schema.HasReference && _supportsRef && schema.HasNonRefContent))
            {
                _success = true;
                _resolvedSchema = schema;

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