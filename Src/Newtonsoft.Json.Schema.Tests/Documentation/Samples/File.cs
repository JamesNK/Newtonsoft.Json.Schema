#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Text;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples
{
    public static class File
    {
        public static StreamReader OpenText(string path)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
        }

        public static StreamWriter CreateText(string path)
        {
            return new StreamWriter(new MemoryStream());
        }

        public static void WriteAllText(string path, string contents)
        {
            Console.WriteLine(contents);
        }

        public static string ReadAllText(string path)
        {
            return "{}";
        }
    }
}