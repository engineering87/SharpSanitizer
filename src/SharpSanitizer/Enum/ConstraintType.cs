// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace SharpSanitizer.Enum
{
    /// <summary>
    /// The supported constraint types.
    /// </summary>
    public enum ConstraintType
    {
        // ---------- Object / Generic ----------
        /// <summary>The object property cannot be NULL.</summary>
        NotNull,
        /// <summary>The object property cannot be DbNull.</summary>
        NoDbNull,

        // ---------- String ----------
        /// <summary>The string must not be null or empty. Null becomes empty.</summary>
        NotNullOrEmpty,
        /// <summary>The string must not be null or whitespace. Null/whitespace becomes empty.</summary>
        NotNullOrWhiteSpace,
        /// <summary>The string length cannot be less than the constraint (pads if needed, per implementation).</summary>
        MinLength,
        /// <summary>The string length cannot be greater than the constraint (truncates if needed).</summary>
        MaxLength,
        /// <summary>The string cannot be null or greater than the constraint (null becomes empty; trims to max).</summary>
        MaxNotNull,
        /// <summary>The string must be uppercase (culture-invariant).</summary>
        Uppercase,
        /// <summary>The string must be lowercase (culture-invariant).</summary>
        Lowercase,
        /// <summary>Removes all whitespace characters from the string.</summary>
        NoWhiteSpace,
        /// <summary>Removes all characters except letters, digits, underscore and dot.</summary>
        NoSpecialCharacters,
        /// <summary>Keeps only digits in the string.</summary>
        OnlyDigit,
        /// <summary>The string must represent a valid date/time (culture-invariant) or becomes empty.</summary>
        ValidDatetime,
        /// <summary>The string must represent a valid date/time; invalid values are forced to MinValue string.</summary>
        ForceToValidDatetime,
        /// <summary>The string must be a single character (trimmed; empty if not possible).</summary>
        SingleChar,
        /// <summary>The string must represent a valid GUID.</summary>
        ValidGuid,
        /// <summary>The string must represent a valid e-mail.</summary>
        ValidEmail,

        // ---------- Numeric ----------
        /// <summary>The number must not be greater than the constraint (upper clamp).</summary>
        MaxValue,
        /// <summary>The number must not be less than the constraint (lower clamp).</summary>
        MinValue,
        /// <summary>For signed numbers, negative values are clamped to zero.</summary>
        NotNegative,
        /// <summary>The number must be &gt;= 0 (equivalent to NotNegative for signed types).</summary>
        Positive,
        /// <summary>The number must be &gt; 0 (strictly positive; zero is adjusted per implementation).</summary>
        StrictPositive,
        /// <summary>The number must not be zero (adjustment or exception per severity/implementation).</summary>
        NonZero,
        /// <summary>Rounds (or truncates, per implementation) to the specified number of decimal places.</summary>
        RoundTo,
        /// <summary>Limits the number of decimal places (typically by truncation).</summary>
        MaxDecimalPlaces,

        // ---------- Collections ----------
        /// <summary>The collection must not be empty (at least one element).</summary>
        NotEmptyCollection,
        /// <summary>The collection elements must be distinct (duplicates removed as per implementation).</summary>
        DistinctCollection
    }
}