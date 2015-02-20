#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal class ReleaseDateAttribute : Attribute
    {
        private readonly DateTime _releaseDate;

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
        }

        public ReleaseDateAttribute(string releaseDate)
        {
            _releaseDate = DateTime.ParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}