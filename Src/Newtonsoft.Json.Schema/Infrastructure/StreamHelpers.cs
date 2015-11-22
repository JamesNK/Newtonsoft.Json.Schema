#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class StreamHelpers
    {
#if NET35
        public static void CopyTo(this Stream source, Stream destination)
        {
            source.CopyTo(destination, 81920);
        }

        public static void CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            int readCount;
            byte[] buffer = new byte[bufferSize];
            while ((readCount = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, readCount);
            }
        }
#endif
    }
}