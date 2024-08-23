// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;

namespace SharpSanitizer.Test.Model
{
    /// <summary>
    /// Simple class for testing purpose
    /// </summary>
    public class FooModel
    {
        /// <summary>
        /// String properties
        /// </summary>
        public string StringMaxNotNull { get; set; }
        public string StringMax { get; set; }
        public string StringNoWhiteSpace { get; set; }
        public string StringNoSpecialCharacters { get; set; }
        public string StringOnlyDigit { get; set; }
        public string ValidDatetime { get; set; }
        public string ForceToValidDatetime { get; set; }
        public string StringSingleChar { get; set; }
        public string StringValidGuid { get; set; }
        /// <summary>
        /// Integer properties
        /// </summary>
        public int MinIntegerProperty { get; set; }
        public int MaxIntegerProperty { get; set; }
        /// <summary>
        /// List properties
        /// </summary>
        public List<string> ListPropertyNotNull { get; set; }
        /// <summary>
        /// Decimal properties
        /// </summary>
        public decimal DecimalMaxDecimalsPlaces { get; set; }
        /// <summary>
        /// Double properties
        /// </summary>
        public double DoubleMaxDecimalsPlaces { get; set; }
        /// <summary>
        /// Object properties
        /// </summary>
        public object ObjectPropertyNoDbNull { get; set; }
    }
}
