// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpSanitizer.Enum;

namespace SharpSanitizer.Entity
{
    /// <summary>
    /// Class that corresponds to a specific constraint to be applied.
    /// </summary>
    public sealed class Constraint
    {
        public ConstraintType ConstraintType;

        public ConstraintValue ConstraintValue;

        /// <summary>
        /// Creates a new constraint without specifing the type.
        /// </summary>
        /// <param name="constraintType"></param>
        public Constraint(ConstraintType constraintType)
        {
            ConstraintType = constraintType;
        }

        /// <summary>
        /// Creates a new constraint of a specific type and with a reference value.
        /// </summary>
        /// <param name="constraintType">The constraint type</param>
        /// <param name="refValue">The reference value for the constraint</param>
        public Constraint(ConstraintType constraintType, int refValue)
        {
            ConstraintType = constraintType;
            ConstraintValue = new ConstraintValue(refValue);
        }
    }
}
