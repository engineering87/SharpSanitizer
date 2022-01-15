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
        public string StringMaxNotNull { get; set; }
        public string StringMax { get; set; }
        public string StringNoWhiteSpace { get; set; }
        public string StringNoSpecialCharacters { get; set; }
        public int MinIntegerProperty { get; set; }
        public int MaxIntegerProperty { get; set; }
        public List<string> ListProperty { get; set; }
    }
}
