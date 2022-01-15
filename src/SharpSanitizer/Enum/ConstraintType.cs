// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpSanitizer.Enum
{
    /// <summary>
    /// The supported constraint types.
    /// </summary>
    public enum ConstraintType
    {
        ///<summary>The object property cannot be NULL</summary>
        NotNull,
        ///<summary>The string property length cannot be greater than the constraint</summary>
        Max,
        ///<summary>The string property cannot be NULL or greater than the constraint</summary>
        MaxNotNull,
        ///<summary>The integer property cannot be less than the constraint</summary>
        Min,
        ///<summary>The integer property cannot be negative</summary>
        NotNegative,
        ///<summary>The string property must be uppercase</summary>
        Uppercase,
        ///<summary>The string property must be lowercase</summary>
        Lowercase,
        ///<summary>The string property cannot contain spaces</summary>
        NoWhiteSpace,
        ///<summary>The string property cannot contain special characters</summary>
        NoSpecialCharacters
    }
}
