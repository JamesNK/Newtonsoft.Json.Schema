﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal static class LicenseHelpers
    {
        private static readonly object Lock;

        private static long _validationCount;
        private static long _generationCount;
        private static LicenseDetails? _registeredLicense;
        private static Timer? _resetTimer;

        static LicenseHelpers()
        {
            Lock = new object();
        }

        internal static void ResetCounts(object state)
        {
            lock (Lock)
            {
                _validationCount = 0;
                _generationCount = 0;
            }
        }

        public static void IncrementAndCheckValidationCount()
        {
            if (_registeredLicense != null)
            {
                return;
            }

            lock (Lock)
            {
                EnsureResetTimer();

                const int maxOperationCount = 1000;
                _validationCount++;

                if (_validationCount > maxOperationCount)
                {
                    throw new JSchemaException("The free-quota limit of {0} schema validations per hour has been reached. Please visit http://www.newtonsoft.com/jsonschema to upgrade to a commercial license.".FormatWith(CultureInfo.InvariantCulture, maxOperationCount));
                }
            }
        }

        public static void IncrementAndCheckGenerationCount()
        {
            if (_registeredLicense != null)
            {
                return;
            }

            lock (Lock)
            {
                EnsureResetTimer();

                const int maxOperationCount = 10;
                _generationCount++;

                if (_generationCount > maxOperationCount)
                {
                    throw new JSchemaException("The free-quota limit of {0} schema generations per hour has been reached. Please visit http://www.newtonsoft.com/jsonschema to upgrade to a commercial license.".FormatWith(CultureInfo.InvariantCulture, maxOperationCount));
                }
            }
        }

        private static void EnsureResetTimer()
        {
            if (_resetTimer == null)
            {
                int interval = Convert.ToInt32(TimeSpan.FromHours(1).TotalMilliseconds);
                Timer timer = new Timer(ResetCounts, null, interval, interval);

                _resetTimer = timer;
            }
        }

        internal static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static bool HasRegisteredLicense()
        {
            return _registeredLicense != null;
        }

        public static void RegisterLicense(string license)
        {
            ReleaseDateAttribute releaseDateAttribute = ReflectionUtils.GetAttribute<ReleaseDateAttribute>(typeof(LicenseHelpers).Assembly())!;

            RegisterLicense(license, releaseDateAttribute.ReleaseDate);
        }

        internal static void RegisterLicense(string license, DateTime releaseDate)
        {
            string licenseBase64;
            int licenseId;
            SplitLicense(license, out licenseBase64, out licenseId);

            lock (Lock)
            {
                if (_registeredLicense != null && _registeredLicense.Id == licenseId)
                {
                    return;
                }

                LicenseDetails deserializedLicense = ReadLicenseData(releaseDate, licenseBase64, licenseId);

                SetRegisteredLicense(deserializedLicense);
            }
        }

        private static LicenseDetails ReadLicenseData(DateTime releaseDate, string licenseBase64, int licenseId)
        {
            byte[] licenseData;

            try
            {
                licenseData = Convert.FromBase64String(licenseBase64);
            }
            catch
            {
                throw new JSchemaException("Specified license text is invalid.");
            }

            if (licenseData.Length <= 128)
            {
                throw new JSchemaException("Specified license text is invalid.");
            }

            // Unfortunately there isn't a clean way to figure out the signature length. Guess based on overall data length and fallback.
            LicenseDetails? deserializedLicense;
            byte[]? signature;
            if (licenseData.Length > 256 && TryGetLicenseAndSignature(keyLength: 256, licenseData, out deserializedLicense, out signature))
            {
                if (!CryptographyHelpers.ValidateData(deserializedLicense.GetSignificateData(), signature, HashAlgorithm.SHA256))
                {
                    throw new JSchemaException("License text does not match signature.");
                }
            }
            else if (TryGetLicenseAndSignature(keyLength: 128, licenseData, out deserializedLicense, out signature))
            {
                if (!CryptographyHelpers.ValidateData(deserializedLicense.GetSignificateData(), signature, HashAlgorithm.SHA1))
                {
                    throw new JSchemaException("License text does not match signature.");
                }
            }
            else
            {
                throw new JSchemaException("Error parsing license text.");
            }

            if (deserializedLicense.Id != licenseId)
            {
                throw new JSchemaException("License ID does not match signature license ID.");
            }

            if (deserializedLicense.ExpiryDate < releaseDate)
            {
                string message = "License is not valid for this version of Json.NET Schema. License free upgrade date expired on {0}. This version of Json.NET Schema was released on {1}.".FormatWith(
                    CultureInfo.InvariantCulture,
                    deserializedLicense.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    releaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                throw new JSchemaException(message);
            }

            const int unitTestLicenseId = 1002;
            if (deserializedLicense.Type == LicenseType.Test && (deserializedLicense.ExpiryDate <= DateTime.UtcNow || deserializedLicense.Id == unitTestLicenseId))
            {
                string message = "Specified test license expiried on {0}.".FormatWith(
                    CultureInfo.InvariantCulture,
                    deserializedLicense.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                throw new JSchemaException(message);
            }

            return deserializedLicense;
        }

        private static bool TryGetLicenseAndSignature(int keyLength, byte[] licenseData, [NotNullWhen(true)] out LicenseDetails? deserializedLicense, [NotNullWhen(true)] out byte[]? signature)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(licenseData, keyLength, licenseData.Length - keyLength))
                using (JsonTextReader reader = new JsonTextReader(new StreamReader(ms)))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    deserializedLicense = serializer.Deserialize<LicenseDetails>(reader)!;
                }
                signature = SubArray(licenseData, 0, keyLength);
                return true;
            }
            catch
            {
                deserializedLicense = null;
                signature = null;
                return false;
            }
        }

        private static void SplitLicense(string license, out string licenseBase64, out int licenseId)
        {
            if (string.IsNullOrEmpty(license))
            {
                throw new JSchemaException("License text is empty.");
            }

            string[] licenseParts = license.Trim().Split('-');
            if (licenseParts.Length != 2)
            {
                throw new JSchemaException("Specified license text is invalid.");
            }

            if (!int.TryParse(licenseParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out licenseId))
            {
                throw new JSchemaException("Specified license text is invalid.");
            }

            licenseBase64 = licenseParts[1];
        }

        private static void SetRegisteredLicense(LicenseDetails license)
        {
            _registeredLicense = license;
            if (_resetTimer != null)
            {
                _resetTimer.Dispose();
                _resetTimer = null;
            }
        }
    }
}