#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Generation
{
    public abstract class JSchemaGenerationProvider
    {
        public abstract JSchema GetSchema(JSchemaTypeGenerationContext context);
    }
}