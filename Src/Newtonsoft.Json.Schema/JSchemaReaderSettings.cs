using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies the settings used when reading a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaReaderSettings
    {
        /// <summary>
        /// The base URI for the schema being read. The base URI is used to resolve relative URI schema references.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// The <see cref="JSchemaResolver"/> to use when resolving schema references.
        /// </summary>
        public JSchemaResolver Resolver { get; set; }
    }
}
