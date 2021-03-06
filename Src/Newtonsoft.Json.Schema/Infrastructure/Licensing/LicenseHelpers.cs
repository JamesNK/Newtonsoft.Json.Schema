#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                Timer timer = new(ResetCounts, null, 0, Convert.ToInt32(TimeSpan.FromHours(1).TotalMilliseconds));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

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
            SplitLicense(license, out string licenseBase64, out int licenseId);

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

            LicenseDetails deserializedLicense;

            using (MemoryStream ms = new(licenseData, 128, licenseData.Length - 128))
            using (JsonTextReader reader = new(new StreamReader(ms)))
            {
                JsonSerializer serializer = new();

                deserializedLicense = serializer.Deserialize<LicenseDetails>(reader)!;
            }

            byte[] data = deserializedLicense.GetSignificateData();
            byte[] signature = SubArray(licenseData, 0, 128);

            if (!CryptographyHelpers.ValidateData(data, signature))
            {
                throw new JSchemaException("License text does not match signature.");
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