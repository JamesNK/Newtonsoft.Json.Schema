#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !NET20
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class NullableDateTimeTestClass
    {
        public string PreField { get; set; }
        public DateTime? DateTimeField { get; set; }
        public DateTimeOffset? DateTimeOffsetField { get; set; }
        public string PostField { get; set; }
    }
}

#endif