#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Tests.TestObjects
{
    public class Store
    {
        public StoreColor Color = StoreColor.Yellow;
        public DateTime Establised = new DateTime(2010, 1, 22, 1, 1, 1, DateTimeKind.Utc);
        public double Width = 1.1;
        public int Employees = 999;
        public int[] RoomsPerFloor = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public bool Open = false;
        public char Symbol = '@';

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<string> Mottos = new List<string>();

        public decimal Cost = 100980.1M;
        public string Escape = "\r\n\t\f\b?{\\r\\n\"\'";

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<Product> product = new List<Product>();

        public Store()
        {
            Mottos.Add("Hello World");
            Mottos.Add("öäüÖÄÜ\\'{new Date(12345);}[222]_µ@²³~");
            Mottos.Add(null);
            Mottos.Add(" ");

            Product rocket = new Product
            {
                Name = "Rocket",
                ExpiryDate = new DateTime(2000, 2, 2, 23, 1, 30, DateTimeKind.Utc)
            };
            Product alien = new Product
            {
                Name = "Alien"
            };

            product.Add(rocket);
            product.Add(alien);
        }
    }
}