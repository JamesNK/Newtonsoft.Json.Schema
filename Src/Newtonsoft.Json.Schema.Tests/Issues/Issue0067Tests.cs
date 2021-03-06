#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0067Tests : TestFixtureBase
    {
        private readonly string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""lineItems"": {
      ""type"": ""array"",
      ""items"": {
        ""properties"": {
          ""placementNumber"": {
            ""maxLength"": 40,
            ""type"": ""string""
          },
          ""site"": {
            ""maxLength"": 60,
            ""type"": ""string""
          },
          ""dealCode"": {
            ""maxLength"": 3,
            ""type"": ""string""
          },
          ""estimatedUniqueViews"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""maxFileSize"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""maxLooping"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""supplierProductName"": {
            ""maxLength"": 256,
            ""type"": ""string""
          },
          ""creativeTypeDescription"": {
            ""maxLength"": 40,
            ""type"": ""string""
          },
          ""supplierSalesAdserverOID"": {
            ""maxLength"": 20,
            ""type"": ""string""
          },
          ""placementUrl"": {
            ""maxLength"": 256,
            ""type"": ""string""
          },
          ""maxAnimationTiming"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""marketValue"": {
            ""maxLength"": 250,
            ""type"": ""string""
          },
          ""target"": {
            ""maxLength"": 1024,
            ""type"": ""string""
          },
          ""supplerSalesOrderRef"": {
            ""maxLength"": 20,
            ""type"": ""string""
          },
          ""estimatedPageViews"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""shareOfVoice"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""sizeNumCols"": {
            ""maxLength"": 9,
            ""type"": ""string""
          },
          ""inventoryType"": {
            ""maxLength"": 30,
            ""type"": ""string""
          },
          ""flashVersion"": {
            ""maxLength"": 12,
            ""type"": ""string""
          },
          ""supplierSalesOrderRefVersion"": {
            ""maxLength"": 10,
            ""type"": ""string""
          },
          ""comments"": {
            ""maxLength"": 1024,
            ""type"": ""string""
          },
          ""sizeNumUnits"": {
            ""maxLength"": 9,
            ""type"": ""string""
          },
          ""supplierPlacementParentRef"": {
            ""maxLength"": 50,
            ""type"": ""string""
          },
          ""supplierPlacementRef"": {
            ""maxLength"": 20,
            ""type"": ""string""
          },
          ""unitAmount"": {
            ""type"": ""integer""
          },
          ""packageId"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.0"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.1"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.2"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.3"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.4"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.5"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.6"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.7"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.8"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.9"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.10"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.11"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.12"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.13"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.14"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.15"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.16"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.17"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.18"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.19"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.20"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.21"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.22"": {
            ""type"": ""integer""
          },
          ""expandedFlighting.23"": {
            ""type"": ""integer""
          },
          ""rate"": {
            ""maxLength"": 16,
            ""type"": ""number"",
            ""multipleOf"": 0.0
          },
          ""plannedCost"": {
            ""maxLength"": 20,
            ""type"": ""number""
          },
          ""marketRate"": {
            ""maxLength"": 16,
            ""type"": ""number""
          },
          ""flightStart"": {
            ""type"": ""string""
          },
          ""flightEnd"": {
            ""type"": ""string""
          },
          ""coverDate"": {
            ""type"": ""string""
          },
          ""supplierOrderDate"": {
            ""type"": ""string""
          },
          ""creativeDueDate"": {
            ""type"": ""string""
          },
          ""saleDate"": {
            ""type"": ""string""
          },
          ""supplierValidityEndDate"": {
            ""type"": ""string""
          },
          ""copyDeadline"": {
            ""type"": ""string""
          },
          ""primaryPlacement"": {
            ""type"": ""boolean""
          },
          ""placementName"": {
            ""type"": ""string""
          },
          ""mediaProperty"": {
            ""type"": ""string""
          },
          ""costMethod"": {
            ""type"": ""string""
          },
          ""unitType"": {
            ""type"": ""string""
          },
          ""placementType"": {
            ""type"": ""string""
          },
          ""buyCategory"": {
            ""type"": ""string""
          },
          ""dimensionsCombinedWithPosition"": {
            ""type"": ""string""
          },
          ""size"": {
            ""type"": ""string""
          },
          ""servedBy"": {
            ""type"": ""string""
          },
          ""color"": {
            ""type"": ""string""
          },
          ""printPosition"": {
            ""type"": ""string""
          },
          ""richMediaAccepted"": {
            ""type"": ""string""
          },
          ""creativeType"": {
            ""type"": ""string""
          },
          ""region"": {
            ""type"": ""string""
          },
          ""currencyType"": {
            ""type"": ""string""
          },
          ""guranteed"": {
            ""type"": ""string""
          },
          ""includeInDigitalEdition"": {
            ""type"": ""string""
          },
          ""dimensionsPosition"": {
            ""type"": ""string""
          },
          ""subsection"": {
            ""type"": ""string""
          },
          ""dimensions"": {
            ""type"": ""string""
          },
          ""section"": {
            ""type"": ""string""
          },
          ""richMediaFeeIncluded"": {
            ""type"": ""string""
          },
          ""mediaType"": {
            ""type"": ""string""
          }
        }
      }
    }
  },
  ""required"": [
    ""lineItems""
  ]
}";

        [Test]
        public void Test()
        {
            ExceptionAssert.Throws<JSchemaReaderException>(() =>
            {
                JSchema.Parse(schemaJson);
            }, "multipleOf must be greater than zero. Path 'properties.lineItems.items.properties.rate.multipleOf', line 186, position 29.");
        }
    }
}