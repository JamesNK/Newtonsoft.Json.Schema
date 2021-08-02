#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Generation
{
    internal class TypeSchemaKey : IEquatable<TypeSchemaKey>
    {
        public readonly Type Type;
        public readonly Required Required;
        public readonly int? MinLength;
        public readonly int? MaxLength;
        public readonly string? Title;
        public readonly string? Description;

        public TypeSchemaKey(
            Type type,
            Required required,
            int? minLength,
            int? maxLength,
            string? title,
            string? description
            )
        {
            Type = type;
            Required = required;
            MinLength = minLength;
            MaxLength = maxLength;
            Title = title;
            Description = description;
        }

        public bool Equals(TypeSchemaKey? other)
        {
            if (other == null)
            {
                return false;
            }

            if (Type != other.Type)
            {
                return false;
            }
            if (Required != other.Required)
            {
                return false;
            }
            if (MinLength != other.MinLength)
            {
                return false;
            }
            if (MaxLength != other.MaxLength)
            {
                return false;
            }
            if (Title != other.Title)
            {
                return false;
            }
            if (Description != other.Description)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            TypeSchemaKey? key = obj as TypeSchemaKey;
            return Equals(key);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            if (Type != null)
            {
                hashCode ^= Type.GetHashCode();
            }
            hashCode ^= Required.GetHashCode();
            if (MinLength != null)
            {
                hashCode ^= MinLength.GetHashCode();
            }
            if (MaxLength != null)
            {
                hashCode ^= MaxLength.GetHashCode();
            }
            if (Title != null)
            {
#if NET5_0_OR_GREATER
                hashCode ^= Title.GetHashCode(StringComparison.Ordinal);
#else
                hashCode ^= Title.GetHashCode();
#endif
            }
            if (Description != null)
            {
#if NET5_0_OR_GREATER
                hashCode ^= Description.GetHashCode(StringComparison.Ordinal);
#else
                hashCode ^= Description.GetHashCode();
#endif
            }

            return hashCode;
        }
    }

    internal class TypeSchema
    {
        public readonly TypeSchemaKey Key;
        public readonly JSchema Schema;

        public TypeSchema(TypeSchemaKey key, JSchema schema)
        {
            Key = key;
            Schema = schema;
        }
    }
}