using System;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema
{
    internal class TypeSchemaKey : IEquatable<TypeSchemaKey>
    {
        public readonly Type Type;
        public readonly Required Required;
        //public readonly string Pattern;
        //public readonly string Format;
        public readonly int? MinLength;
        public readonly int? MaxLength;
        //public readonly JToken MinValue;
        //public readonly JToken MaxValue;
        //public readonly Type EnumType;

        public TypeSchemaKey(
            Type type,
            Required required,
            //string pattern,
            //string format,
            int? minLength,
            int? maxLength
            //JToken minValue,
            //JToken maxValue,
            //Type enumType
            )
        {
            Type = type;
            Required = required;
            //Pattern = pattern;
            //Format = format;
            MinLength = minLength;
            MaxLength = maxLength;
            //MinValue = minValue;
            //MaxValue = maxValue;
            //EnumType = enumType;
        }

        public bool Equals(TypeSchemaKey other)
        {
            if (other == null)
                return false;

            if (Type != other.Type)
                return false;
            if (Required != other.Required)
                return false;
            //if (Pattern != other.Pattern)
            //    return false;
            //if (Format != other.Format)
            //    return false;
            if (MinLength != other.MinLength)
                return false;
            if (MaxLength != other.MaxLength)
                return false;
            //if (MinValue != other.MinValue)
            //    return false;
            //if (MaxValue != other.MaxValue)
            //    return false;
            //if (EnumType != other.EnumType)
            //    return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            TypeSchemaKey key = obj as TypeSchemaKey;
            return Equals(key);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            if (Type != null)
                hashCode ^= Type.GetHashCode();
            hashCode ^= Required.GetHashCode();
            //if (Pattern != null)
            //    hashCode ^= Pattern.GetHashCode();
            //if (Format != null)
            //    hashCode ^= Format.GetHashCode();
            if (MinLength != null)
                hashCode ^= MinLength.GetHashCode();
            if (MaxLength != null)
                hashCode ^= MaxLength.GetHashCode();
            //if (MinValue != null)
            //    hashCode ^= MinValue.GetHashCode();
            //if (MaxValue != null)
            //    hashCode ^= MaxValue.GetHashCode();
            //if (EnumType != null)
            //    hashCode ^= EnumType.GetHashCode();

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