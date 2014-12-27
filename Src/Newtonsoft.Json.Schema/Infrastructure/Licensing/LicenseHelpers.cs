#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal static class LicenseHelpers
    {
        private static readonly object Lock;

        private static long _operationCount;
        private static LicenseDetails _registeredLicense;
        private static Timer _operationResetTimer;

        static LicenseHelpers()
        {
            Lock = new object();
        }

        internal static void ResetOperationCount(object state)
        {
            _operationCount = 0;
        }

        public static void IncrementAndCheckOperationCount()
        {
            if (_registeredLicense != null)
                return;

            if (_operationResetTimer == null)
            {
                lock (Lock)
                {
                    if (_operationResetTimer == null)
                    {
                        _operationResetTimer = new Timer(ResetOperationCount, null, 0, Convert.ToInt32(TimeSpan.FromHours(1).TotalMilliseconds));
                    }
                }
            }

            const int maxOperationCount = 1000;
            Interlocked.Increment(ref _operationCount);

            if (_operationCount > maxOperationCount)
                throw new JsonException("The free-quota limit of {0} schema validations per hour has been reached. Please visit http://www.newtonsoft.com to upgrade to a commercial license.".FormatWith(CultureInfo.InvariantCulture, maxOperationCount));
        }

        internal static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void RegisterLicense(string license)
        {
            ReleaseDateAttribute releaseDateAttribute = ReflectionUtils.GetAttribute<ReleaseDateAttribute>(typeof(LicenseHelpers).Assembly());

            RegisterLicense(license, releaseDateAttribute.ReleaseDate);
        }

        internal static void RegisterLicense(string license, DateTime releaseDate)
        {
            if (string.IsNullOrWhiteSpace(license))
                throw new JsonException("License text is empty.");

            string[] licenseParts = license.Trim().Split('-');
            if (licenseParts.Length != 2)
                throw new JsonException("Specified license text is invalid.");

            int licenseId;
            if (!int.TryParse(licenseParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out licenseId))
                throw new JsonException("Specified license text is invalid.");

            byte[] licenseData;

            try
            {
                licenseData = Convert.FromBase64String(licenseParts[1]);
            }
            catch
            {
                throw new JsonException("Specified license text is invalid.");
            }

            if (licenseData.Length <= 128)
                throw new JsonException("Specified license text is invalid.");

            MemoryStream ms = new MemoryStream(licenseData, 0, licenseData.Length - 128);
            JsonSerializer serializer = new JsonSerializer();
            
            LicenseDetails deserializedLicense = serializer.Deserialize<LicenseDetails>(new JsonTextReader(new StreamReader(ms)));

            byte[] data = deserializedLicense.GetSignificateData();
            byte[] signature = SubArray(licenseData, licenseData.Length - 128, 128);

            if (!CryptographyHelpers.ValidateData(data, signature))
                throw new JsonException("License text does not match signature.");

            if (deserializedLicense.Id != licenseId)
                throw new JsonException("License ID does not match signature license ID.");

            if (deserializedLicense.ExpiryDate < releaseDate)
            {
                string message = "License is not valid for this version of Json.NET Schema. License expired on {0}. This version of Json.NET Schema was released on {1}.".FormatWith(
                    CultureInfo.InvariantCulture,
                    deserializedLicense.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    releaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                throw new JsonException(message);
            }

            if (deserializedLicense.Type == LicenseType.Test)
                throw new JsonException("Specified license is for testing only.");

            SetRegisteredLicense(deserializedLicense);
        }

        private static void SetRegisteredLicense(LicenseDetails license)
        {
            _registeredLicense = license;
            if (_operationResetTimer != null)
            {
                _operationResetTimer.Dispose();
                _operationResetTimer = null;
            }
        }
    }
}