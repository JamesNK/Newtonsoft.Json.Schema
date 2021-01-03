#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class CompositeContext : ContextBase, ISchemaTracker
    {
        public List<ContextBase> Contexts { get; }

        public CompositeContext(Validator validator) : base(validator)
        {
            Contexts = new List<ContextBase>();
        }

        public override bool HasErrors
        {
            get
            {
                for (int i = 0; i < Contexts.Count; i++)
                {
                    if (Contexts[i].HasErrors)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
        {
            for (int i = 0; i < Contexts.Count; i++)
            {
                Contexts[i].RaiseError(message, errorType, schema, value, childErrors);
            }
        }

        public List<JSchema> EvaluatedSchemas => throw new NotSupportedException();
 
        public bool TrackEvaluatedSchemas
        {
            get
            {
                for (int i = 0; i < Contexts.Count; i++)
                {
                    if (Contexts[i] is ISchemaTracker schemaTracker && schemaTracker.TrackEvaluatedSchemas)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void TrackEvaluatedSchema(JSchema schema)
        {
            for (int i = 0; i < Contexts.Count; i++)
            {
                if (Contexts[i] is ISchemaTracker schemaTracker)
                {
                    schemaTracker.TrackEvaluatedSchema(schema);
                }
            }
        }
    }
}