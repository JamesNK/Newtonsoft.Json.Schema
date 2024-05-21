#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Security.Cryptography;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal enum HashAlgorithm
    {
        SHA1,
        SHA256
    }

    internal static class CryptographyHelpers
    {
        private const string PublicKey = "<RSAKeyValue><Modulus>wNE8tiipWCy2LmB3cZYW8nj5Nm/fn3X2GYsoSx6XE1yfvW96Ul/vRBw6/jAAwk9aZIdix9+gleh5x7XE8snzZlNMDDCmIFz2SWY9f7SdYYD5gif2rIpeeIDS/5J731d6XX/BKISwtM+MRWakY6ihNU1SUIGsKH6HxUXPm80Q66s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const string PublicKeyCsp = "BgIAAACkAABSU0ExAAQAAAEAAQCr6xDNm89FxYd+KKyBUFJNNaGoY6RmRYzPtLCEKMF/XXpX33uS/9KAeF6KrPYngvmAYZ20fz1mSfZcIKYwDExTZvPJ8sS1x3nolaDfx2KHZFpPwgAw/jocRO9fUnpvvZ9cE5ceSyiLGfZ1n99vNvl48haWcXdgLrYsWKkotjzRwA==";

        private const string PublicKey2048Xml = "<RSAKeyValue><Modulus>vTcWi0k75jLKRKN1eqkDfDuCIFfhhXFDFFKW9BnjIsWBzfl3lvDFC/ev8m6Aimz3QNZEbZMX8e0uPipzo9BaxQNn3k5NCb64Y7lww8KbwKiiQI6yZOOHf/ImX5yUI5H3w1DeMf9Nx5ccBOIh1vFfAHiDhGK7dgLOfw2rRPTGk46RgNZJgqMOoDQEeM10VcJpHi4hDs5nvhptSXVIv3kkSfUPNgeJ70HPG+eyq2hwOlCMj/c9SQCc69tyiY3twdStDP6ikIDu8A0T921BzJbANSLS7IoQUySkviIkNDUnOn5QyZqJS3oUgR94ObP9DFew8x1WBgVpPdfbx4mcNXCh0Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const string PublicKey2048Blob = "MIIBCgKCAQEAvTcWi0k75jLKRKN1eqkDfDuCIFfhhXFDFFKW9BnjIsWBzfl3lvDFC/ev8m6Aimz3QNZEbZMX8e0uPipzo9BaxQNn3k5NCb64Y7lww8KbwKiiQI6yZOOHf/ImX5yUI5H3w1DeMf9Nx5ccBOIh1vFfAHiDhGK7dgLOfw2rRPTGk46RgNZJgqMOoDQEeM10VcJpHi4hDs5nvhptSXVIv3kkSfUPNgeJ70HPG+eyq2hwOlCMj/c9SQCc69tyiY3twdStDP6ikIDu8A0T921BzJbANSLS7IoQUySkviIkNDUnOn5QyZqJS3oUgR94ObP9DFew8x1WBgVpPdfbx4mcNXCh0QIDAQAB";

        internal static bool ValidateData(byte[] data, byte[] signature, HashAlgorithm hashAlgorithm)
        {
#if NETSTANDARD2_1_OR_GREATER
            using (RSA rsa = RSA.Create())
            {
                HashAlgorithmName hashAlgorithmName;
                switch (hashAlgorithm)
                {
                    default:
                    case HashAlgorithm.SHA1:
                        rsa.ImportParameters(ToRSAParameters(Convert.FromBase64String(PublicKeyCsp), false));
                        hashAlgorithmName = HashAlgorithmName.SHA1;
                        break;
                    case HashAlgorithm.SHA256:
                        rsa.ImportRSAPublicKey(Convert.FromBase64String(PublicKey2048Blob), out int bytesRead);
                        hashAlgorithmName = HashAlgorithmName.SHA256;
                        break;
                }

                return rsa.VerifyData(data, signature, hashAlgorithmName, RSASignaturePadding.Pkcs1);
            }
#elif NETSTANDARD2_0_OR_GREATER
            if (hashAlgorithm != HashAlgorithm.SHA1)
            {
                throw new JSchemaException("License hash algorithm is not supported on this platform: " + hashAlgorithm);
            }

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(ToRSAParameters(Convert.FromBase64String(PublicKeyCsp), false));

                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
#else
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider())
            {
                IDisposable algorithmProvider;
                switch (hashAlgorithm)
                {
                    default:
                    case HashAlgorithm.SHA1:
                        algorithmProvider = new SHA1CryptoServiceProvider();
                        rsaCryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(PublicKeyCsp));
                        break;
                    case HashAlgorithm.SHA256:
                        algorithmProvider = new SHA256CryptoServiceProvider();
                        rsaCryptoServiceProvider.FromXmlString(PublicKey2048Xml);
                        break;
                }

                try
                {
                    return rsaCryptoServiceProvider.VerifyData(data, algorithmProvider, signature);
                }
                finally
                {
                    algorithmProvider.Dispose();
                }
            }
#endif
        }

#if NETSTANDARD2_0_OR_GREATER
        internal const int ALG_TYPE_RSA = (2 << 9);
        internal const int ALG_CLASS_KEY_EXCHANGE = (5 << 13);
        internal const int CALG_RSA_KEYX = (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_RSA | 0);

        internal static RSAParameters ToRSAParameters(this byte[] cspBlob, bool includePrivateParameters)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(cspBlob));

            byte bType = br.ReadByte(); // BLOBHEADER.bType: Expected to be 0x6 (PUBLICKEYBLOB) or 0x7 (PRIVATEKEYBLOB), though there's no check for backward compat reasons. 
            byte bVersion = br.ReadByte(); // BLOBHEADER.bVersion: Expected to be 0x2, though there's no check for backward compat reasons.
            br.ReadUInt16(); // BLOBHEADER.wReserved
            int algId = br.ReadInt32(); // BLOBHEADER.aiKeyAlg
            if (algId != CALG_RSA_KEYX)
                throw new PlatformNotSupportedException(); // The FCall this code was ported from supports other algid's but we're only porting what we use.

            int magic = br.ReadInt32(); // RSAPubKey.magic: Expected to be 0x31415352 ('RSA1') or 0x32415352 ('RSA2') 
            int bitLen = br.ReadInt32(); // RSAPubKey.bitLen

            int modulusLength = bitLen/8;
            int halfModulusLength = (modulusLength + 1)/2;

            uint expAsDword = br.ReadUInt32();

            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Exponent = ExponentAsBytes(expAsDword);
            rsaParameters.Modulus = br.ReadReversed(modulusLength);
            if (includePrivateParameters)
            {
                rsaParameters.P = br.ReadReversed(halfModulusLength);
                rsaParameters.Q = br.ReadReversed(halfModulusLength);
                rsaParameters.DP = br.ReadReversed(halfModulusLength);
                rsaParameters.DQ = br.ReadReversed(halfModulusLength);
                rsaParameters.InverseQ = br.ReadReversed(halfModulusLength);
                rsaParameters.D = br.ReadReversed(modulusLength);
            }

            return rsaParameters;
        }

        /// <summary>
        /// Helper for converting a UInt32 exponent to bytes.
        /// </summary>
        private static byte[] ExponentAsBytes(uint exponent)
        {
            if (exponent <= 0xFF)
            {
                return new[] {(byte) exponent};
            }
            if (exponent <= 0xFFFF)
            {
                return new[]
                {
                    (byte) (exponent >> 8),
                    (byte) (exponent)
                };
            }
            if (exponent <= 0xFFFFFF)
            {
                return new[]
                {
                    (byte) (exponent >> 16),
                    (byte) (exponent >> 8),
                    (byte) (exponent)
                };
            }

            return new[]
            {
                (byte) (exponent >> 24),
                (byte) (exponent >> 16),
                (byte) (exponent >> 8),
                (byte) (exponent)
            };
        }

        /// <summary>
        /// Read in a byte array in reverse order.
        /// </summary>
        private static byte[] ReadReversed(this BinaryReader br, int count)
        {
            byte[] data = br.ReadBytes(count);
            Array.Reverse(data);
            return data;
        }
#endif
    }
}