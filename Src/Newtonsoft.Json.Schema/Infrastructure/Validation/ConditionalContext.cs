#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ConditionalContext : ContextBase, ISchemaTracker
    {
        private readonly ISchemaTracker _parentSchemaTracker;

        public List<ValidationError> Errors;
        public List<JSchema> EvaluatedSchemas { get; private set; }
        public bool TrackEvaluatedSchemas { get; }

        public ConditionalContext(Validator validator, ContextBase parentContext, bool trackEvaluatedSchemas)
            : base(validator)
        {
            _parentSchemaTracker = parentContext as ISchemaTracker;

            // Track evaluated schemas if requested, or the parent context is already tracking.
            TrackEvaluatedSchemas = trackEvaluatedSchemas || (_parentSchemaTracker?.TrackEvaluatedSchemas ?? false);
        }

        public override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            if (Errors == null)
            {
                Errors = new List<ValidationError>();
            }

            Errors.Add(Validator.CreateError(message, errorType, schema, value, childErrors));
        }

        public void TrackEvaluatedSchema(JSchema schema)
        {
            if (TrackEvaluatedSchemas)
            {
                // If a parent is available then only store schemas in parent for efficency
                if (_parentSchemaTracker != null && _parentSchemaTracker.TrackEvaluatedSchemas)
                {
                    _parentSchemaTracker.TrackEvaluatedSchema(schema);
                }
                else
                {
                    if (EvaluatedSchemas == null)
                    {
                        EvaluatedSchemas = new List<JSchema>();
                    }

                    EvaluatedSchemas.Add(schema);
                }
            }
        }

        public static ConditionalContext Create(ContextBase context, bool trackEvaluatedSchemas)
        {
            return new ConditionalContext(context.Validator, context, trackEvaluatedSchemas);
        }

        public override bool HasErrors => !Errors.IsNullOrEmpty();
    }
}