using System;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class DummyJSchema4Resolver : JSchema4Resolver
    {
        public static readonly DummyJSchema4Resolver Instance = new DummyJSchema4Resolver();

        public override JSchema4 GetSchema(Uri uri)
        {
            return null;
        }
    }
}