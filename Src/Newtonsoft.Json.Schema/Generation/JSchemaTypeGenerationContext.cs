using System;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Generation
{
    public class JSchemaTypeGenerationContext
    {
        private readonly Type _objectType;
        private readonly Required _required;
        private readonly JsonProperty _memberProperty;
        private readonly JsonContainerContract _parentContract;
        private readonly JSchemaGenerator _generator;

        public Type ObjectType
        {
            get { return _objectType; }
        }

        public JsonProperty MemberProperty
        {
            get { return _memberProperty; }
        }

        public JsonContainerContract ParentContract
        {
            get { return _parentContract; }
        }

        public Required Required
        {
            get { return _required; }
        }

        public JSchemaGenerator Generator
        {
            get { return _generator; }
        }

        public JSchemaTypeGenerationContext(Type objectType, Required required, JsonProperty memberProperty, JsonContainerContract parentContract, JSchemaGenerator generator)
        {
            _objectType = objectType;
            _required = required;
            _memberProperty = memberProperty;
            _parentContract = parentContract;
            _generator = generator;
        }
    }
}