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

        private static long _validationCount;
        private static long _generationCount;
        private static LicenseDetails _registeredLicense;
        private static Timer _resetTimer;

        static LicenseHelpers()
        {
            Lock = new object();
        }

        internal static void ResetCounts(object state)
        {
            _validationCount = 0;
            _generationCount = 0;
        }

        public static void IncrementAndCheckValidationCount()
        {
            if (_registeredLicense != null)
                return;

            EnsureResetTimer();

            const int maxOperationCount = 1000;
            Interlocked.Increment(ref _validationCount);

            if (_validationCount > maxOperationCount)
                throw new JSchemaException("The free-quota limit of {0} schema validations per hour has been reached. Please visit http://www.newtonsoft.com/jsonschema to upgrade to a commercial license.".FormatWith(CultureInfo.InvariantCulture, maxOperationCount));
        }

        public static void IncrementAndCheckGenerationCount()
        {
            if (_registeredLicense != null)
                return;

            EnsureResetTimer();

            const int maxOperationCount = 10;
            Interlocked.Increment(ref _generationCount);

            if (_generationCount > maxOperationCount)
                throw new JSchemaException("The free-quota limit of {0} schema generations per hour has been reached. Please visit http://www.newtonsoft.com/jsonschema to upgrade to a commercial license.".FormatWith(CultureInfo.InvariantCulture, maxOperationCount));
        }

        private static void EnsureResetTimer()
        {
            if (_resetTimer == null)
            {
                lock (Lock)
                {
                    if (_resetTimer == null)
                    {
                        Timer timer = new Timer(ResetCounts, null, 0, Convert.ToInt32(TimeSpan.FromHours(1).TotalMilliseconds));

#if !PORTABLE
                        Thread.MemoryBarrier();
#endif
                        _resetTimer = timer;
                    }
                }
            }
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
                throw new JSchemaException("License text is empty.");

            string[] licenseParts = license.Trim().Split('-');
            if (licenseParts.Length != 2)
                throw new JSchemaException("Specified license text is invalid.");

            int licenseId;
            if (!int.TryParse(licenseParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out licenseId))
                throw new JSchemaException("Specified license text is invalid.");

            byte[] licenseData;

            try
            {
                licenseData = Convert.FromBase64String(licenseParts[1]);
            }
            catch
            {
                throw new JSchemaException("Specified license text is invalid.");
            }

            if (licenseData.Length <= 128)
                throw new JSchemaException("Specified license text is invalid.");

            MemoryStream ms = new MemoryStream(licenseData, 128, licenseData.Length - 128);
            JsonSerializer serializer = new JsonSerializer();
            
            LicenseDetails deserializedLicense = serializer.Deserialize<LicenseDetails>(new JsonTextReader(new StreamReader(ms)));

            byte[] data = deserializedLicense.GetSignificateData();
            byte[] signature = SubArray(licenseData, 0, 128);

            if (!CryptographyHelpers.ValidateData(data, signature))
                throw new JSchemaException("License text does not match signature.");

            if (deserializedLicense.Id != licenseId)
                throw new JSchemaException("License ID does not match signature license ID.");

            if (deserializedLicense.ExpiryDate < releaseDate)
            {
                string message = "License is not valid for this version of Json.NET Schema. License expired on {0}. This version of Json.NET Schema was released on {1}.".FormatWith(
                    CultureInfo.InvariantCulture,
                    deserializedLicense.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    releaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                throw new JSchemaException(message);
            }

            if (deserializedLicense.Type == LicenseType.Test)
                throw new JSchemaException("Specified license is for testing only.");

            SetRegisteredLicense(deserializedLicense);
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