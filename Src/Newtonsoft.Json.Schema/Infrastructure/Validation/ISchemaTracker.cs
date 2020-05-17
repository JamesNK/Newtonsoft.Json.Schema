#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal interface ISchemaTracker
    {
        void TrackEvaluatedSchema(JSchema schema);
        List<JSchema> EvaluatedSchemas { get; }
        bool TrackEvaluatedSchemas { get; }
    }
}