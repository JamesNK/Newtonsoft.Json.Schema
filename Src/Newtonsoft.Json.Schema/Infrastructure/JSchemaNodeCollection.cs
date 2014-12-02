#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.ObjectModel;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaNodeCollection : KeyedCollection<string, JSchemaNode>
    {
        protected override string GetKeyForItem(JSchemaNode item)
        {
            return item.Id;
        }
    }
}