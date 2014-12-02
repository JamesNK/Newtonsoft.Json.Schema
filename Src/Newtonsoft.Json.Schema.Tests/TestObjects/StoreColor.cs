#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    [Flags]
    public enum StoreColor
    {
        Black = 1,
        Red = 2,
        Yellow = 4,
        White = 8,
        DarkGoldenrod = 16
    }
}