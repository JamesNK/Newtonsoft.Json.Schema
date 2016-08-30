#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Generation
{
    internal class JSchemaGeneratorProxy : JSchemaGenerator
    {
        private readonly JSchemaGenerator _generator;
        private readonly JSchemaGeneratorInternal _generatorInternal;
        private readonly JSchemaGenerationProvider _generationProvider;

        public JSchemaGeneratorProxy(JSchemaGeneratorInternal generatorInternal, JSchemaGenerationProvider generationProvider)
        {
            _generator = generatorInternal._generator;
            _generatorInternal = generatorInternal;
            _generationProvider = generationProvider;
        }

        public override IContractResolver ContractResolver
        {
            get { return _generator.ContractResolver; }
            set { _generator.ContractResolver = value; }
        }

        public override Required DefaultRequired
        {
            get { return _generator.DefaultRequired; }
            set { _generator.DefaultRequired = value; }
        }

        public override IList<JSchemaGenerationProvider> GenerationProviders
        {
            get { return _generator.GenerationProviders; }
        }

        public override SchemaIdGenerationHandling SchemaIdGenerationHandling
        {
            get { return _generator.SchemaIdGenerationHandling; }
            set { _generator.SchemaIdGenerationHandling = value; }
        }

        public override SchemaLocationHandling SchemaLocationHandling
        {
            get { return _generator.SchemaLocationHandling; }
            set { _generator.SchemaLocationHandling = value; }
        }

        public override SchemaPropertyOrderHandling SchemaPropertyOrderHandling
        {
            get { return _generator.SchemaPropertyOrderHandling; }
            set { _generator.SchemaPropertyOrderHandling = value; }
        }

        public override SchemaReferenceHandling SchemaReferenceHandling
        {
            get { return _generator.SchemaReferenceHandling; }
            set { _generator.SchemaReferenceHandling = value; }
        }

        public override JSchema Generate(Type type)
        {
            return Generate(type, false);
        }

        public override JSchema Generate(Type type, bool rootSchemaNullable)
        {
            Required required = rootSchemaNullable ? Required.AllowNull : Required.Always;

            // the current generation provider will not be called to avoid stackoverflow
            return _generatorInternal.GenerateSubschema(type, required, _generationProvider);
        }
    }
}
