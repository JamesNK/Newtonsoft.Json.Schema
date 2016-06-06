#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using System.Text;
using System.Threading;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#elif DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using XAssert = Xunit.Assert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Utilities;
using System.Collections;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
#if DNXCORE50
        protected TestFixtureBase()
#else
        [SetUp]
        protected void TestSetup()
#endif
        {
            LicenseHelpers.ResetCounts(null);
        }

        protected void WriteEscapedJson(string json)
        {
            Console.WriteLine(EscapeJson(json));
        }

        protected string EscapeJson(string json)
        {
            return @"@""" + json.Replace(@"""", @"""""") + @"""";
        }
    }

    public static class CustomAssert
    {
        public static void IsInstanceOfType(Type t, object instance)
        {
#if NETFX_CORE
            if (!instance.GetType().IsAssignableFrom(t))
                throw new Exception("Not instance of type");
#else
            Assert.IsInstanceOf(t, instance);
#endif
        }

        public static void Contains(IList collection, object value)
        {
            Contains(collection, value, null);
        }

        public static void Contains(IList collection, object value, string message)
        {
#if !(NETFX_CORE || DNXCORE50)
            Assert.Contains(value, collection, message);
#else
            if (!collection.Cast<object>().Any(i => i.Equals(value)))
                throw new Exception(message ?? "Value not found in collection.");
#endif
        }
    }

#if DNXCORE50
    public class TestFixtureAttribute : Attribute
    {
        // xunit doesn't need a test fixture attribute
        // this exists so the project compiles
    }

    public class XUnitAssert
    {
        public static void IsInstanceOf(Type expectedType, object o)
        {
            XAssert.IsType(expectedType, o);
        }

        public static void AreEqual(double expected, double actual, double r)
        {
            XAssert.Equal(expected, actual, 5); // hack
        }

        public static void AreEqual(object expected, object actual, string message = null)
        {
            XAssert.Equal(expected, actual);
        }

        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            XAssert.Equal(expected, actual);
        }

        public static void AreNotEqual(object expected, object actual, string message = null)
        {
            XAssert.NotEqual(expected, actual);
        }

        public static void AreNotEqual<T>(T expected, T actual, string message = null)
        {
            XAssert.NotEqual(expected, actual);
        }

        public static void Fail(string message = null, params object[] args)
        {
            if (message != null)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args);
            }

            XAssert.True(false, message);
        }

        public static void Pass()
        {
        }

        public static void IsTrue(bool condition, string message = null, params object[] args)
        {
            XAssert.True(condition);
        }

        public static void IsFalse(bool condition, string message = null, params object[] args)
        {
            XAssert.False(condition);
        }

        public static void IsNull(object o)
        {
            XAssert.Null(o);
        }

        public static void IsNotNull(object o)
        {
            XAssert.NotNull(o);
        }

        public static void AreNotSame(object expected, object actual)
        {
            XAssert.NotSame(expected, actual);
        }

        public static void AreSame(object expected, object actual)
        {
            XAssert.Same(expected, actual);
        }
    }

    public class CollectionAssert
    {
        public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            XAssert.Equal(expected, actual);
        }

        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            XAssert.Equal(expected, actual);
        }

        public static void Contains<T>(IEnumerable<T> values, T expected)
        {
            XAssert.Contains(expected, values);
        }
    }
#endif

#if DNXCORE50
    public static class Console
    {
        public static void WriteLine(params object[] args)
        {
        }
    }
#endif

    public static class StringAssert
    {
        private readonly static Regex Regex = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.CultureInvariant);

        public static void AreEqual(string expected, string actual)
        {
            expected = Normalize(expected);
            actual = Normalize(actual);

            if (expected != actual)
            {
                Console.WriteLine("Actual content:");
                Console.WriteLine(EscapeText(actual) ?? "{null}");
                Console.WriteLine();
            }

            Assert.AreEqual(expected, actual);
        }

        private static string EscapeText(string t)
        {
            if (t == null)
            {
                return null;
            }

            return @"@""" + t.Replace(@"""", @"""""") + @"""";
        }

        public static bool Equals(string s1, string s2)
        {
            s1 = Normalize(s1);
            s2 = Normalize(s2);

            return string.Equals(s1, s2);
        }

        public static string Normalize(string s)
        {
            if (s != null)
            {
                s = Regex.Replace(s, "\r\n");
            }

            return s;
        }
    }

    public static class ExceptionAssert
    {
        public static void Throws<TException>(Action action, params string[] possibleMessages)
            where TException : Exception
        {
            try
            {
                action();

                Assert.Fail("Exception of type {0} expected. No exception thrown.", typeof(TException).Name);
            }
            catch (TException ex)
            {
                if (possibleMessages != null && possibleMessages.Length > 0)
                {
                    bool match = false;
                    foreach (string possibleMessage in possibleMessages)
                    {
                        if (StringAssert.Equals(possibleMessage, ex.Message))
                        {
                            match = true;
                            break;
                        }
                    }

                    if (!match)
                    {
                        throw new Exception("Unexpected exception message." + Environment.NewLine + "Expected one of: " + string.Join(Environment.NewLine, possibleMessages) + Environment.NewLine + "Got: " + ex.Message + Environment.NewLine + Environment.NewLine + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception of type {0} expected; got exception of type {1}.", typeof(TException).Name, ex.GetType().Name), ex);
            }
        }
    }
}