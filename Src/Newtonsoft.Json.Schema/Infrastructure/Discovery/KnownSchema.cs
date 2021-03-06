#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("{" + nameof(Id) + "}")]
    internal class KnownSchema
    {
        public readonly Uri Id;
        public readonly JSchema Schema;

        public KnownSchemaState State;

        public KnownSchema(Uri id, JSchema schema, KnownSchemaState state)
        {
            Id = id;
            Schema = schema;
            State = state;
        }
    }
}