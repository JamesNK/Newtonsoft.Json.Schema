#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Tests;
using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Json.NET Schema Test Console");

            string jsonNetVersion = FileVersionInfo.GetVersionInfo(typeof(JsonConvert).Assembly.Location).FileVersion;
            string schemaVersion = FileVersionInfo.GetVersionInfo(typeof(JSchema).Assembly.Location).FileVersion;
            Console.WriteLine("Json.NET Version: " + jsonNetVersion);
            Console.WriteLine("Json.NET Schema Version: " + schemaVersion);
            Console.ReadKey();

            Console.WriteLine("Doing stuff...");

            ValidateJson();

            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static void ValidateJson()
        {
            PerformanceTests t = new PerformanceTests();
            t.IsValidPerformance_Failure();
        }
    }
}