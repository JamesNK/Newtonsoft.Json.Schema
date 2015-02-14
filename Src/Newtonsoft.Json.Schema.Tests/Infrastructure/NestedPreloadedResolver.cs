using System;
using System.Linq;

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    public class NestedPreloadedResolver : JSchemaPreloadedResolver
    {
        public override SchemaReference ResolveSchemaReference(ResolveSchemaContext context)
        {
            SchemaReference reference = base.ResolveSchemaReference(context);

            if (PreloadedUris.Any(i => i == reference.BaseUri))
                return reference;

            foreach (Uri preloadedUri in PreloadedUris)
            {
                if (preloadedUri.IsBaseOf(reference.BaseUri))
                {
                    Uri relativeUri = preloadedUri.MakeRelativeUri(reference.BaseUri);

                    string uriText = relativeUri.OriginalString;
                    if (reference.SubschemaId != null)
                        uriText += reference.SubschemaId.OriginalString;

                    reference.BaseUri = preloadedUri;
                    reference.SubschemaId = new Uri(uriText, UriKind.RelativeOrAbsolute);

                    return reference;
                }   
            }

            return reference;
        }

        //public override Uri ResolveBaseUri(ResolveSchemaContext context)
        //{
        //    Uri uri = base.ResolveBaseUri(context);
        //    if (PreloadedUris.Any(i => i == uri))
        //        return uri;

        //    foreach (Uri preloadedUri in PreloadedUris)
        //    {
        //        if (preloadedUri.IsBaseOf(uri))
        //            return preloadedUri;
        //    }

        //    return uri;
        //}
    }
}