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
        ///<summary>The string property length cannot be less than the constraint</summary>
        MinLength,
        ///<summary>The string property length cannot be greater than the constraint</summary>
        MaxLength,
        ///<summary>The string property cannot be NULL or greater than the constraint</summary>
        MaxNotNull,
        ///<summary>The integer property cannot be greater than the constraint</summary>
        MaxValue,
        ///<summary>The integer property cannot be less than the constraint</summary>
        MinValue,
        ///<summary>The integer property cannot be negative</summary>
        NotNegative,
        ///<summary>The string property must be uppercase</summary>
        Uppercase,
        ///<summary>The string property must be lowercase</summary>
        Lowercase,
        ///<summary>The string property cannot contain spaces</summary>
        NoWhiteSpace,
        ///<summary>The string property cannot contain special characters</summary>
        NoSpecialCharacters,
        ///<summary>The string property must contains only digits</summary>
        OnlyDigit,
        ///<summary>The number property must have a limited number of decimals places</summary>
        MaxDecimalPlaces,
        ///<summary>The object property cannot be DbNull</summary>
        NoDbNull,
        ///<summary>The string property is a valid Datetime if parsed</summary>
        ValidDatetime,
        ///<summary>The string property is a valid Datetime if parsed, forced to the MinValue</summary>
        ForceToValidDatetime,
        ///<summary>The string property must be a single char</summary>
        SingleChar,
        ///<summary>The string property represent a valid Guid</summary>
        ValidGuid,
        ///<summary>The string property represent a valid e-mail</summary>
        ValidEmail
    }
}
