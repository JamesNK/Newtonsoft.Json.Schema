#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class Product
    {
        public string Name;
        public DateTime ExpiryDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public decimal Price;
        public string[] Sizes;

        public override bool Equals(object obj)
        {
            if (obj is Product p)
            {
                return (p.Name == Name && p.ExpiryDate == ExpiryDate && p.Price == Price);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Name ?? string.Empty).GetHashCode();
        }
    }
}