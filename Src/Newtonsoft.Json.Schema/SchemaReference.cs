﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Describes a schema reference.
    /// </summary>
    public class SchemaReference
    {
        /// <summary>
        /// The base URI for the referenced schema.
        /// </summary>
        public Uri? BaseUri { get; set; }

        /// <summary>
        /// The subschema ID for the referenced schema.
        /// </summary>
        public Uri? SubschemaId { get; set; }
    }
}