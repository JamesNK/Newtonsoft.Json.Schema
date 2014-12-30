#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Reflection;
#if !PORTABLE
using System.Security.Cryptography;
#endif

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal static class CryptographyHelpers
    {
        private const string PublicKey = "<RSAKeyValue><Modulus>wNE8tiipWCy2LmB3cZYW8nj5Nm/fn3X2GYsoSx6XE1yfvW96Ul/vRBw6/jAAwk9aZIdix9+gleh5x7XE8snzZlNMDDCmIFz2SWY9f7SdYYD5gif2rIpeeIDS/5J731d6XX/BKISwtM+MRWakY6ihNU1SUIGsKH6HxUXPm80Q66s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal static bool ValidateData(byte[] data, byte[] signature)
        {
            bool valid;

#if PORTABLE
            // todo: support WinRT
            Type rsaCryptoServiceProviderType = Type.GetType("System.Security.Cryptography.RSACryptoServiceProvider");
            MethodInfo fromXmlStringMethod = rsaCryptoServiceProviderType.GetTypeInfo().BaseType.GetTypeInfo().GetDeclaredMethod("FromXmlString");
            MethodInfo verifyDataMethod = rsaCryptoServiceProviderType.GetTypeInfo().GetDeclaredMethod("VerifyData");
            Type sha1CryptoServiceProviderType = Type.GetType("System.Security.Cryptography.SHA1CryptoServiceProvider");

            object rsaCryptoServiceProvider = Activator.CreateInstance(rsaCryptoServiceProviderType);

            fromXmlStringMethod.Invoke(rsaCryptoServiceProvider, new object[] { PublicKey });

            valid = (bool)verifyDataMethod.Invoke(rsaCryptoServiceProvider, new object[] { data, Activator.CreateInstance(sha1CryptoServiceProviderType), signature });
#else
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.FromXmlString(PublicKey);

            valid = rsaCryptoServiceProvider.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
#endif

            return valid;
        }
    }
}