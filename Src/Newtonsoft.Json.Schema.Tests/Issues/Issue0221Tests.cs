#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0221Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            JSchema schema = JSchema.Parse(JsonSchema);
            JObject o = JObject.Parse(Data);

            // Act
            bool valid = o.IsValid(schema, out IList<ValidationError> errors);

            // Assert
            Assert.IsFalse(valid);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 1.", errors[0].Message);
            Assert.AreEqual(1, errors[0].ChildErrors.Count);
            Assert.AreEqual("JSON does not match schema from 'then'.", errors[0].ChildErrors[0].Message);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errors[0].ChildErrors[0].ChildErrors[0].Message);
        }

        private const string Data = @"{
    ""FechaEmisionDocumento"": ""2020-05-20T11:00:55.723Z"",
    ""FechaRecepcionDocumento"": ""2020-05-20T11:00:00.178Z"",
    ""FilesConvocatoria"": [],
    ""FilesPresupuestos"": [],
    ""FilesLiquidaciones"": [],
    ""FilesFacturas"": [],
    ""FilesAcuerdo"": [],
    ""TipoDocumento"": ""ConvocatoriaOrd"",
    ""PerimetroPrimario"": false,
    ""ImporteTotalDerrama"": 0,
    ""ImporteMaximoUR"": 0,
    ""SoloURSareb"": false,
    ""ValidarAccion"": ""ConCriteriosAsistencia""
}";

        private const string JsonSchema = @"{
  ""definitions"": {
    ""enums"": {
      ""$id"": ""#/definitions/enums"",
      ""type"": ""object"",
      ""properties"": {
        ""TipoDocumento"": {
          ""$id"": ""#/definitions/enums/properties/TipoDocumento"",
          ""type"": ""string"",
          ""enum"": [
            ""ConvocatoriaOrd"",
            ""ConvocatoriaExt""
          ]
        }
      }
    },
    ""file"": {
      ""$id"": ""#/definitions/file"",
      ""type"": ""object"",
      ""title"": ""The File Schema"",
      ""required"": [],
      ""properties"": {
        ""id"": {
          ""$id"": ""#/definitions/file/properties/id"",
          ""type"": ""integer"",
          ""title"": ""The Id Schema"",
          ""minLength"": 1
        },
        ""alias"": {
          ""$id"": ""#/definitions/file/properties/alias"",
          ""type"": ""string"",
          ""title"": ""The Alias Schema"",
          ""minLength"": 1
        }
      }
    },
    ""ConvocatoriaBase"": {
      ""$id"": ""#/definitions/ConvocatoriaBase"",
      ""type"": ""object"",
      ""title"": ""The ConvocatoriaBase Schema"",
      ""required"": [
        ""TipoDocumento"",
        ""FechaEmisionDocumento"",
        ""FechaRecepcionDocumento""
      ],
      ""properties"": {
        ""TipoDocumento"": {
          ""$id"": ""#/properties/TipoDocumento"",
          ""$ref"": ""#/definitions/enums/properties/TipoDocumento"",
          ""title"": ""The TipoDocumento Schema""
        },
        ""FechaEmisionDocumento"": {
          ""$id"": ""#/properties/FechaEmisionDocumento"",
          ""type"": ""string"",
          ""title"": ""The FechaEmisionDocumento Schema"",
          ""format"": ""date-time""
        },
        ""FechaRecepcionDocumento"": {
          ""$id"": ""#/properties/FechaRecepcionDocumento"",
          ""type"": ""string"",
          ""title"": ""The FechaRecepcionDocumento Schema"",
          ""format"": ""date-time""
        },
        ""Comentarios"": {
          ""$id"": ""#/properties/Comentarios"",
          ""type"": ""string"",
          ""title"": ""The Comentarios Schema""
        },
        ""CesionDeEspacios"": {
          ""$id"": ""#/properties/CesionDeEspacios"",
          ""type"": ""boolean"",
          ""title"": ""The CesionDeEspacios Schema""
        },
        ""TemasReputacionales"": {
          ""$id"": ""#/properties/TemasReputacionales"",
          ""type"": ""boolean"",
          ""title"": ""The TemasReputacionales Schema""
        },
        ""Okupas"": {
          ""$id"": ""#/properties/Okupas"",
          ""type"": ""boolean"",
          ""title"": ""The Okupas Schema""
        },
        ""JuntaImpulsadaPropiedad"": {
          ""$id"": ""#/properties/JuntaImpulsadaPropiedad"",
          ""type"": ""boolean"",
          ""title"": ""The JuntaImpulsadaPropiedad Schema""
        },
        ""JuntaDeConstitucion"": {
          ""$id"": ""#/properties/JuntaDeConstitucion"",
          ""type"": ""boolean"",
          ""title"": ""The JuntaDeConstitucion Schema""
        },
        ""ConvivenciaInquilinos"": {
          ""$id"": ""#/properties/ConvivenciaInquilinos"",
          ""type"": ""boolean"",
          ""title"": ""The ConvivenciaInquilinos Schema""
        },
        ""AfectacionEstructuraEdificio"": {
          ""$id"": ""#/properties/AfectacionEstructuraEdificio"",
          ""type"": ""boolean"",
          ""title"": ""The AfectacionEstructuraEdificio Schema""
        },
        ""ObrasEInstalaciones"": {
          ""$id"": ""#/properties/ObrasEInstalaciones"",
          ""type"": ""boolean"",
          ""title"": ""The ObrasEInstalaciones Schema""
        },
        ""AprobacionDeDerramas"": {
          ""$id"": ""#/properties/AprobacionDeDerramas"",
          ""type"": ""boolean"",
          ""title"": ""The AprobacionDeDerramas Schema""
        },
        ""ImporteDerrama"": {
          ""$id"": ""#/properties/ImporteDerrama"",
          ""title"": ""The ImporteDerrama Schema"",
          ""anyOf"": [
            {
              ""type"": ""number""
            },
            {
              ""type"": ""null""
            }
          ]
        },
        ""PropietarioActuaComoPresidente"": {
          ""$id"": ""#/properties/PropietarioActuaComoPresidente"",
          ""type"": ""boolean"",
          ""title"": ""The PropietarioActuaComoPresidente Schema""
        },
        ""ParticipacionPropiedad100x100"": {
          ""$id"": ""#/properties/ParticipacionPropiedad100x100"",
          ""type"": ""boolean"",
          ""title"": ""The ParticipacionPropiedad100x100 Schema""
        },
        ""CoeficienteParticipacionComunidad"": {
          ""$id"": ""#/properties/CoeficienteParticipacionComunidad"",
          ""title"": ""The CoeficienteParticipacionComunidad Schema"",
          ""anyOf"": [
            {
              ""type"": ""number""
            },
            {
              ""type"": ""null""
            }
          ]
        },
        ""CoeficienteDeGastos"": {
          ""$id"": ""#/properties/CoeficienteDeGastos"",
          ""title"": ""The CoeficienteDeGastos Schema"",
          ""anyOf"": [
            {
              ""type"": ""number""
            },
            {
              ""type"": ""null""
            }
          ]
        },
        ""Observaciones"": {
          ""$id"": ""#/properties/Observaciones"",
          ""type"": ""string"",
          ""title"": ""The Observaciones Schema""
        },
        ""FilesConvocatoria"": {
          ""$id"": ""#/properties/FilesConvocatoria"",
          ""type"": ""array"",
          ""title"": ""The FilesConvocatoria Schema"",
          ""items"": {
            ""$ref"": ""#/definitions/file""
          }
        },
        ""FilesPresupuestos"": {
          ""$id"": ""#/properties/FilesPresupuestos"",
          ""type"": ""array"",
          ""title"": ""The FilesPresupuestos Schema"",
          ""items"": {
            ""$ref"": ""#/definitions/file""
          }
        },
        ""FilesLiquidaciones"": {
          ""$id"": ""#/properties/FilesLiquidaciones"",
          ""type"": ""array"",
          ""title"": ""The FilesLiquidaciones Schema"",
          ""items"": {
            ""$ref"": ""#/definitions/file""
          }
        },
        ""FilesFacturas"": {
          ""$id"": ""#/properties/FilesFacturas"",
          ""type"": ""array"",
          ""title"": ""The FilesFacturas Schema"",
          ""items"": {
            ""$ref"": ""#/definitions/file""
          }
        },
        ""FilesAcuerdo"": {
          ""$id"": ""#/properties/FilesAcuerdo"",
          ""type"": ""array"",
          ""title"": ""The FilesAcuerdo Schema"",
          ""items"": {
            ""$ref"": ""#/definitions/file""
          }
        }
      },
      ""allOf"": [
        {
          ""if"": {
            ""required"": [
              ""AprobacionDeDerramas""
            ],
            ""properties"": {
              ""AprobacionDeDerramas"": {
                ""const"": true
              }
            }
          },
          ""then"": {
            ""required"": [
              ""ImporteDerrama"",
              ""CoeficienteParticipacionComunidad"",
              ""CoeficienteDeGastos""
            ],
            ""properties"": {
              ""ImporteDerrama"": {
                ""type"": ""number""
              },
              ""CoeficienteParticipacionComunidad"": {
                ""type"": ""number"",
                ""minimum"": 0,
                ""maximum"": 100
              },
              ""CoeficienteDeGastos"": {
                ""type"": ""number"",
                ""minimum"": 0,
                ""maximum"": 100
              }
            }
          }
        }
      ]
    },
    ""criteriosAsistenciaFromConvocatoria"": {
      ""$id"": ""#/definitions/criteriosAsistenciaFromConvocatoria"",
      ""type"": ""object"",
      ""title"": ""The CriteriosAsistencia Schema"",
      ""anyOf"": [
        {
          ""required"": [
            ""JuntaDeConstitucion""
          ],
          ""properties"": {
            ""JuntaDeConstitucion"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""DerechoAVoto""
          ],
          ""properties"": {
            ""DerechoAVoto"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""CesionDeEspacios""
          ],
          ""properties"": {
            ""CesionDeEspacios"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""TemasReputacionales""
          ],
          ""properties"": {
            ""TemasReputacionales"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""Okupas""
          ],
          ""properties"": {
            ""Okupas"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""ConvivenciaInquilinos""
          ],
          ""properties"": {
            ""ConvivenciaInquilinos"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""ObrasEInstalaciones""
          ],
          ""properties"": {
            ""ObrasEInstalaciones"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""AprobacionDeDerramas""
          ],
          ""properties"": {
            ""AprobacionDeDerramas"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""PerimetroPrimario""
          ],
          ""properties"": {
            ""PerimetroPrimario"": {
              ""const"": true
            }
          }
        },
        {
          ""required"": [
            ""ImporteTotalDerrama""
          ],
          ""properties"": {
            ""ImporteTotalDerrama"": {
              ""type"": ""number"",
              ""minimum"": 35000
            }
          }
        },
        {
          ""required"": [
            ""ImporteMaximoUR""
          ],
          ""properties"": {
            ""ImporteMaximoUR"": {
              ""type"": ""number"",
              ""minimum"": 7000
            }
          }
        }
      ]
    }
  },
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""act0227schema"",
  ""type"": ""object"",
  ""title"": ""0227-Schema"",
  ""allOf"": [
    {
      ""$ref"": ""#/definitions/ConvocatoriaBase""
    },
    {
      ""if"": {
        ""properties"": {
          ""ValidarAccion"": {
            ""const"": ""ConCriteriosAsistencia""
          }
        },
        ""required"": [
          ""ValidarAccion""
        ]
      },
      ""then"": {
        ""$ref"": ""#/definitions/criteriosAsistenciaFromConvocatoria""
      }
    },
    {
      ""if"": {
        ""properties"": {
          ""ValidarAccion"": {
            ""const"": ""SinCriteriosAsistencia""
          }
        },
        ""required"": [
          ""ValidarAccion""
        ]
      },
      ""then"": {
        ""not"": {
          ""$ref"": ""#/definitions/criteriosAsistenciaFromConvocatoria""
        }
      }
    }
  ]
}";
    }
}
