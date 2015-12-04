#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Reflection;
using System.Linq;
#if !PORTABLE
using System.Security.Cryptography;

#endif

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal static class CryptographyHelpers
    {
        private const string PublicKey = "<RSAKeyValue><Modulus>wNE8tiipWCy2LmB3cZYW8nj5Nm/fn3X2GYsoSx6XE1yfvW96Ul/vRBw6/jAAwk9aZIdix9+gleh5x7XE8snzZlNMDDCmIFz2SWY9f7SdYYD5gif2rIpeeIDS/5J731d6XX/BKISwtM+MRWakY6ihNU1SUIGsKH6HxUXPm80Q66s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const string PublicKeyCsp = "BgIAAACkAABSU0ExAAQAAAEAAQCr6xDNm89FxYd+KKyBUFJNNaGoY6RmRYzPtLCEKMF/XXpX33uS/9KAeF6KrPYngvmAYZ20fz1mSfZcIKYwDExTZvPJ8sS1x3nolaDfx2KHZFpPwgAw/jocRO9fUnpvvZ9cE5ceSyiLGfZ1n99vNvl48haWcXdgLrYsWKkotjzRwA==";

        internal static bool ValidateData(byte[] data, byte[] signature)
        {
            bool valid;

#if PORTABLE
            try
            {
                Type rsaCryptoServiceProviderType = Type.GetType("System.Security.Cryptography.RSACryptoServiceProvider");
                MethodInfo importCspBlobMethod = rsaCryptoServiceProviderType.GetTypeInfo().GetDeclaredMethod("ImportCspBlob");
                MethodInfo verifyDataMethod = rsaCryptoServiceProviderType.GetTypeInfo().GetDeclaredMethod("VerifyData");
                Type sha1CryptoServiceProviderType = Type.GetType("System.Security.Cryptography.SHA1CryptoServiceProvider");

                object rsaCryptoServiceProvider = Activator.CreateInstance(rsaCryptoServiceProviderType);

                importCspBlobMethod.Invoke(rsaCryptoServiceProvider, new object[] { Convert.FromBase64String(PublicKeyCsp) });

                valid = (bool)verifyDataMethod.Invoke(rsaCryptoServiceProvider, new object[] { data, Activator.CreateInstance(sha1CryptoServiceProviderType), signature });
            }
            catch (InvalidOperationException)
            {
                // WinRT - Microsoft why do you do this? STAHP!

                Type asymmetricKeyAlgorithmProviderType = Type.GetType("Windows.Security.Cryptography.Core.AsymmetricKeyAlgorithmProvider, Windows.Security, ContentType=WindowsRuntime");
                MethodInfo openAlgorithmMethod = asymmetricKeyAlgorithmProviderType.GetTypeInfo().GetDeclaredMethod("OpenAlgorithm");
                MethodInfo importPublicKeyMethod = asymmetricKeyAlgorithmProviderType.GetTypeInfo().DeclaredMethods.Single(m => m.Name == "ImportPublicKey" && m.GetParameters().Length == 2);

                Type cryptographicBufferType = Type.GetType("Windows.Security.Cryptography.CryptographicBuffer, Windows.Security, ContentType=WindowsRuntime");
                MethodInfo decodeFromBase64StringMethod = cryptographicBufferType.GetTypeInfo().GetDeclaredMethod("DecodeFromBase64String");
                MethodInfo createFromByteArrayMethod = cryptographicBufferType.GetTypeInfo().GetDeclaredMethod("CreateFromByteArray");

                Type cryptographicEngineType = Type.GetType("Windows.Security.Cryptography.Core.CryptographicEngine, Windows.Security, ContentType=WindowsRuntime");
                MethodInfo verifySignatureMethod = cryptographicEngineType.GetTypeInfo().GetDeclaredMethod("VerifySignature");

                object algorithmProvider = openAlgorithmMethod.Invoke(null, new object[] { "RSASIGN_PKCS1_SHA1" });
                object publicKeyBuffer = decodeFromBase64StringMethod.Invoke(null, new object[] { PublicKeyCsp });
                object publicKey = importPublicKeyMethod.Invoke(algorithmProvider, new object[] { publicKeyBuffer, 3 });
                object dataBuffer = createFromByteArrayMethod.Invoke(null, new object[] { data });
                object signatureBuffer = createFromByteArrayMethod.Invoke(null, new object[] { signature });

                valid = (bool)verifySignatureMethod.Invoke(null, new object[] { publicKey, dataBuffer, signatureBuffer });
            }
#else
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(PublicKeyCsp));

            valid = rsaCryptoServiceProvider.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
#endif

            return valid;
        }
    }
}