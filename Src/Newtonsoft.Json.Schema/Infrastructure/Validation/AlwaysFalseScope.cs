#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion


namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class AlwaysFalseScope : SchemaScope
    {
        internal static AlwaysFalseScope Instance = new AlwaysFalseScope();

        private AlwaysFalseScope()
        {
            IsValid = false;
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            return true;
        }
    }
}