namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Specifies the location of referenced schemas for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    public enum SchemaLocationHandling
    {
        /// <summary>
        /// Referenced schemas are placed in the root schema's definitions collection.
        /// </summary>
        Definitions,
        /// <summary>
        /// Referenced schemas are inline.
        /// </summary>
        Inline
    }
}