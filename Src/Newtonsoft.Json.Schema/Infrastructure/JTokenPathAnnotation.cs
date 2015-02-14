#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JTokenPathAnnotation
    {
        public readonly string BasePath;

        public JTokenPathAnnotation(string basePath)
        {
            BasePath = basePath;
        }
    }
}
