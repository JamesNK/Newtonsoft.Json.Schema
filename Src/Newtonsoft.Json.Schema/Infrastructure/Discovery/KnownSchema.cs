#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("Id = {Id}, DynamicScope = {DynamicScope}, State = {State}, SchemaDebugId = {Schema.DebugId}")]
    internal sealed class KnownSchema
    {
        public readonly Uri Id;
        public readonly Uri? DynamicScope;
        public readonly JSchema Schema;

        public KnownSchemaState State;

        public KnownSchema(Uri id, Uri? dynamicScope, JSchema schema, KnownSchemaState state)
        {
            Id = id;
            DynamicScope = dynamicScope;
            Schema = schema;
            State = state;
        }
    }
}