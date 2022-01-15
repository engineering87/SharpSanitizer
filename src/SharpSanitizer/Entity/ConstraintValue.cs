// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

namespace SharpSanitizer.Entity
{
    /// <summary>
    /// The constraint reference value.
    /// </summary>
    public class ConstraintValue
    {
        private readonly int? Value;

        public int IntegerValue {
            get
            {
                if (Value == null)
                    throw new ArgumentException("The constraint value is NULL or not set");

                return (int)Value;
            }
        }

        public ConstraintValue(int value)
        {
            Value = value;
        }
    }
}
