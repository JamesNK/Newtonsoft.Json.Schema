#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;

#if PORTABLE
[assembly: AssemblyTitle("Json.NET Schema Tests Portable")]
#elif NET40
[assembly: AssemblyTitle("Json.NET Schema Tests .NET 4.0")]
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
[assembly: Guid("a5bb3fb0-90f1-4f9c-a930-dcfed8b8de5d")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.1.18118")]

[assembly: ReleaseDate("2014-12-27")]
