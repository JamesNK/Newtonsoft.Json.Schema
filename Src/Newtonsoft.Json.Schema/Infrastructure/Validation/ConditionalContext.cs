#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    internal class ConditionalContext : ContextBase, ISchemaTracker
    {
        private readonly ISchemaTracker? _parentSchemaTracker;

        public List<ValidationError>? Errors;
        public List<SchemaScope>? EvaluatedSchemas { get; private set; }
        public bool TrackEvaluatedSchemas { get; }

        public ConditionalContext(Validator validator, ContextBase parentContext, bool trackEvaluatedSchemas, bool useParentTracker)
            : base(validator)
        {
            var parentSchemaTracker = parentContext as ISchemaTracker;
            if (useParentTracker)
            {
                _parentSchemaTracker = parentSchemaTracker;
            }

            // Track evaluated schemas if requested, or the parent context is already tracking.
            TrackEvaluatedSchemas = trackEvaluatedSchemas || (parentSchemaTracker?.TrackEvaluatedSchemas ?? false);
        }

        private string DebuggerDisplay()
        {
            var evaluatedSchemas = _parentSchemaTracker?.EvaluatedSchemas ?? EvaluatedSchemas;
            return $"Errors = {Errors?.Count ?? 0}, TrackEvaluatedSchemas = {TrackEvaluatedSchemas}, EvaluatedSchemas = {evaluatedSchemas?.Count ?? 0}";
        }

        public override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
        {
            if (Errors == null)
            {
                Errors = new List<ValidationError>();
            }

            Errors.Add(Validator.CreateError(message, errorType, schema, value, childErrors));
        }

        public void TrackEvaluatedSchemaScope(SchemaScope schema)
        {
            if (TrackEvaluatedSchemas)
            {
                // If a parent is available then only store schemas in parent for efficiency
                if (_parentSchemaTracker != null)
                {
                    _parentSchemaTracker.TrackEvaluatedSchemaScope(schema);
                }
                else
                {
                    if (EvaluatedSchemas == null)
                    {
                        EvaluatedSchemas = new List<SchemaScope>();
                    }

                    EvaluatedSchemas.Add(schema);
                }
            }
        }

        public static ConditionalContext Create(ContextBase context, bool trackEvaluatedSchemas, bool useParentTracker = true)
        {
            return new ConditionalContext(context.Validator, context, trackEvaluatedSchemas, useParentTracker);
        }

        [MemberNotNullWhen(true, nameof(Errors))]
        public override bool HasErrors => !Errors.IsNullOrEmpty();
    }
}