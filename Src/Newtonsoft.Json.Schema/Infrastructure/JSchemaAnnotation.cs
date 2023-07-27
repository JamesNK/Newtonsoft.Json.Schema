#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaAnnotation
    {
        private static readonly Uri NoScope = new Uri("http://noscope");

        private readonly Dictionary<Uri, JSchema> _schemas;

        public JSchemaAnnotation()
        {
            _schemas = new Dictionary<Uri, JSchema>(UriComparer.Instance);
        }

        public bool HasSchemas => _schemas.Count > 0;

        public void RegisterSchema(Uri? dynamicScope, JSchema schema)
        {
            _schemas[dynamicScope ?? NoScope] = schema;
        }

        public JSchema? GetSchema(Uri? dynamicScope)
        {
            _schemas.TryGetValue(dynamicScope ?? NoScope, out JSchema? schema);
            return schema;
        }

        public IEnumerable<KeyValuePair<Uri?, JSchema>> GetSchemas()
        {
            foreach (var item in _schemas)
            {
                if (item.Key == NoScope)
                {
                    yield return new KeyValuePair<Uri?, JSchema>(null, item.Value);
                }
                yield return new KeyValuePair<Uri?, JSchema>(item.Key, item.Value);
            }
        }

        public bool TryGetSingle([NotNullWhen(true)] out JSchema? schema)
        {
            if (_schemas.Count == 1)
            {
                schema = _schemas.Single().Value;
                return true;
            }

            schema = null;
            return false;
        }
    }
}