#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;

#if NETSTANDARD1_3
[assembly: AssemblyTitle("Json.NET Schema Tests .NET Standard 1.3")]
#elif PORTABLE
[assembly: AssemblyTitle("Json.NET Schema Tests Portable")]
#elif NET40
[assembly: AssemblyTitle("Json.NET Schema Tests .NET 4.0")]
#elif NET35
[assembly: AssemblyTitle("Json.NET Schema Tests .NET 3.5")]
#else

[assembly: AssemblyTitle("Json.NET Schema Tests")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Newtonsoft")]
[assembly: AssemblyProduct("Json.NET Schema")]
[assembly: AssemblyCopyright("Copyright © Newtonsoft 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.1.18123")]

// leave unchanged for unit tests
[assembly: ReleaseDate("2014-12-27")]