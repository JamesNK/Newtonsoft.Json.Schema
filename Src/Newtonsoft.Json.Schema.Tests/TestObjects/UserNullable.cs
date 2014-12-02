#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class UserNullable
    {
        public Guid Id;
        public string FName;
        public string LName;
        public int RoleId;
        public int? NullableRoleId;
        public int? NullRoleId;
        public bool? Active;
    }
}