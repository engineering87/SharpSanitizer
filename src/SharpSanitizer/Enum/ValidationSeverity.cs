// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.ComponentModel;

namespace SharpSanitizer.Enum
{
    public enum ValidationSeverity
    {
        [Description("Throws an exception if the data is invalid.")]
        Strict,
        [Description("Try to fix invalid data without raising any error.")]
        Relaxed
    }
}