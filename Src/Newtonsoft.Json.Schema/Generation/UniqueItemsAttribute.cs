using System;

namespace Newtonsoft.Json.Schema.Generation
{

    /// <summary>
    /// Instructs the <see cref="JSchemaGenerator"/> to add unique items is true to the member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter, AllowMultiple = false)]
    public class UniqueItemsAttribute : Attribute
    {
    }
}
