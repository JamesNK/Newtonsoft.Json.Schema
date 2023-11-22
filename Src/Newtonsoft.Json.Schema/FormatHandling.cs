#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies how the <c>format</c> keyword is handled when validating schemas.
    /// </summary>
    public enum FormatHandling
    {
        /// <summary>
        /// The <c>format</c> keyword uses the default behavior for the schema version.
        /// In draft 2019-09 and earlier, failures are asserted, and result in a validation error.
        /// In draft 2020-12 and later, failures are treated as annotations, and don't cause a validation error.
        /// </summary>
        Default,
        /// <summary>
        /// The <c>format</c> keyword is evaluated as an annotation. Newtonsoft.Json.Schema doesn't
        /// support annotations so it skips running the <c>format</c> keyword.
        /// </summary>
        Annotation,
        /// <summary>
        /// The <c>format</c> keyword is evaluated as an assertion. Failure causes a validation error.
        /// </summary>
        Assertion
    }
}