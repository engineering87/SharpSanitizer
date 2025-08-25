// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using SharpSanitizer.Entity;
using SharpSanitizer.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SharpSanitizer
{
    public sealed class SharpSanitizer<T> : ISharpSanitizer<T>
    {
        private readonly Dictionary<string, Constraint> _propertiesConstraints;
        private readonly ValidationSeverity _validationSeverity;

        public SharpSanitizer(Dictionary<string, Constraint> propertiesConstraints)
        {
            _propertiesConstraints = propertiesConstraints;
            _validationSeverity = ValidationSeverity.Relaxed;
        }

        public SharpSanitizer(Dictionary<string, Constraint> propertiesConstraints, ValidationSeverity validationSeverity)
        {
            _propertiesConstraints = propertiesConstraints;
            _validationSeverity = validationSeverity;
        }

        /// <summary>
        /// Sanitizes the properties of the object.
        /// </summary>
        /// <param name="obj">The object to be sanitized</param>
        public void Sanitize(T obj)
        {
            if (obj == null) return;

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                if (!_propertiesConstraints.TryGetValue(pi.Name, out var constraint)) continue;

                var propertyValue = pi.GetValue(obj, null);
                var targetType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                var typeCode = Type.GetTypeCode(targetType);

                switch (typeCode)
                {
                    case TypeCode.String:
                        {
                            var propertyValueSanitize = ApplyStringConstraint(propertyValue?.ToString(), constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        {
                            var propertyValueSanitize = ApplyObjectConstraint(propertyValue, pi.PropertyType, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        break;
                    case TypeCode.Char:
                        break;
                    case TypeCode.SByte:
                        {
                            if (propertyValue == null) break;
                            var orig = (sbyte)propertyValue;
                            var sanitized = (sbyte)ApplyInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            if (propertyValue == null) break;
                            var orig = (byte)propertyValue;
                            var sanitized = (byte)ApplyUInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            if (propertyValue == null) break;
                            var original = (short)propertyValue;
                            var sanitized = ApplyIntegerConstraint(original, constraint);
                            pi.SetValue(obj, (short)sanitized);
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            if (propertyValue == null) break;
                            var orig = (ushort)propertyValue;
                            var sanitized = (ushort)ApplyUInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            if (propertyValue == null) break;
                            var original = (int)propertyValue;
                            var sanitized = ApplyIntegerConstraint(original, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            if (propertyValue == null) break;
                            var orig = (uint)propertyValue;
                            var sanitized = (uint)ApplyUInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            if (propertyValue == null) break;
                            var orig = (long)propertyValue;
                            var sanitized = ApplyInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            if (propertyValue == null) break;
                            var orig = (ulong)propertyValue;
                            var sanitized = ApplyUInt64Constraint(orig, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Single:
                        {
                            if (propertyValue == null) break;
                            var sanitized = (float)ApplyDoubleConstraint((double)(float)propertyValue, constraint);
                            pi.SetValue(obj, sanitized);
                            break;
                        }
                    case TypeCode.Double:
                        {
                            if (propertyValue == null) break;
                            var propertyValueSanitize = ApplyDoubleConstraint((double)propertyValue, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            if (propertyValue == null) break;
                            var propertyValueSanitize = ApplyDecimalConstraint((decimal)propertyValue, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.DateTime:
                        break;
                }
            }
        }

        /// <summary>
        /// Sanitizes the properties of objects within the list.
        /// </summary>
        /// <param name="list">The list to be sanitized</param>
        public void Sanitize(IEnumerable<T> list)
        {
            if (list == null) return;
            foreach (var obj in list)
            {
                Sanitize(obj);
            }
        }

        /// <summary>
        /// Apply the current constraint to the string property.
        /// </summary>
        /// <param name="propertyValue">The original property value.</param>
        /// <param name="constraint">The constraint configuration.</param>
        /// <param name="validationSeverity">The validation severity to apply</param>
        /// <returns></returns>
        private string ApplyStringConstraint(string propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;
            switch (constraint.ConstraintType)
            {
                case ConstraintType.NotNull:
                    return propertyValue?.Trim() ?? string.Empty;
                case ConstraintType.NotNullOrEmpty:
                    return string.IsNullOrEmpty(propertyValue) ? string.Empty : propertyValue.Trim();
                case ConstraintType.NotNullOrWhiteSpace:
                    return string.IsNullOrWhiteSpace(propertyValue) ? string.Empty : propertyValue.Trim();
                case ConstraintType.MaxLength:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        var trimString = propertyValue?.Trim();
                        return trimString?.Length <= constraintRefValue.Value
                            ? trimString : trimString?[..constraintRefValue.Value];
                    }
                case ConstraintType.MinLength:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        var trimString = propertyValue?.Trim();
                        return (trimString?.Length ?? 0) >= constraintRefValue.Value
                            ? trimString
                            : (trimString ?? string.Empty).PadRight(constraintRefValue.Value);
                    }
                case ConstraintType.MaxNotNull:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        if (propertyValue == null) return string.Empty;
                        var trimString = propertyValue?.Trim();
                        return trimString?.Length <= constraintRefValue.Value
                            ? trimString : trimString?.Substring(0, constraintRefValue.Value);
                    }
                case ConstraintType.Lowercase:
                    {
                        if (propertyValue == null) return string.Empty;
                        return propertyValue?.Trim().ToLowerInvariant();
                    }
                case ConstraintType.Uppercase:
                    {
                        if (propertyValue == null) return string.Empty;
                        return propertyValue?.Trim().ToUpperInvariant();
                    }
                case ConstraintType.NoWhiteSpace:
                    {
                        if (propertyValue == null) return string.Empty;
                        return Regex.Replace(propertyValue, @"\s+", "");
                    }
                case ConstraintType.NoSpecialCharacters:
                    {
                        if (propertyValue == null) return string.Empty;
                        return Regex.Replace(propertyValue, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    }
                case ConstraintType.OnlyDigit:
                    {
                        if (propertyValue == null) return string.Empty;
                        return Regex.Replace(propertyValue, @"[^\d]+", "").Trim();
                    }
                case ConstraintType.ValidDatetime:
                    {
                        if (propertyValue == null) return string.Empty;
                        if (DateTime.TryParse(propertyValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                            return propertyValue?.Trim();
                        return string.Empty;
                    }
                case ConstraintType.ForceToValidDatetime:
                    {
                        if (propertyValue == null) return DateTime.MinValue.ToString();
                        if (DateTime.TryParse(propertyValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                            return propertyValue?.Trim();
                        return DateTime.MinValue.ToString();
                    }
                case ConstraintType.SingleChar:
                    {
                        if (propertyValue == null) return string.Empty;
                        var trimString = propertyValue?.Trim();
                        return string.IsNullOrEmpty(trimString) ? string.Empty : trimString.Substring(0, 1);
                    }
                case ConstraintType.ValidGuid:
                    {
                        if (propertyValue == null) return string.Empty;
                        if(Guid.TryParse(propertyValue.Trim(), out _))
                            return propertyValue?.Trim();
                        else
                        {
                            if (_validationSeverity == ValidationSeverity.Strict)
                                throw new ArgumentException($"The property {propertyValue} is not a valid Guid");
                            return Guid.NewGuid().ToString();
                        }                        
                    }
                case ConstraintType.ValidEmail:
                    {
                        if (propertyValue == null) return string.Empty;
                        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                        if (Regex.IsMatch(propertyValue.Trim(), pattern, RegexOptions.IgnoreCase))
                            return propertyValue?.Trim();
                        else
                        {
                            if (_validationSeverity == ValidationSeverity.Strict)
                                throw new ArgumentException($"The property {propertyValue} is not a valid email");
                            return string.Empty;
                        }
                    }
                default:
                    return propertyValue?.Trim();
            }
        }

        /// <summary>
        /// Apply the current constraint to the integer property.
        /// </summary>
        /// <param name="propertyValue">The original property value.</param>
        /// <param name="constraint">The constraint configuration.</param>
        /// <returns></returns>
        private int ApplyIntegerConstraint(int propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;
            switch (constraint.ConstraintType)
            {
                case ConstraintType.MinValue:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return Math.Max(propertyValue, constraintRefValue.Value);
                    }
                case ConstraintType.MaxValue:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return Math.Min(propertyValue, constraintRefValue.Value);
                    }
                case ConstraintType.NotNegative:
                    {
                        return Math.Max(propertyValue, 0);
                    }
                case ConstraintType.Positive:
                    {
                        return Math.Max(propertyValue, 0);
                    }
                case ConstraintType.StrictPositive:
                    {
                        return propertyValue > 0 ? propertyValue : 1;
                    }
                case ConstraintType.NonZero:
                    {
                        if (propertyValue != 0) return propertyValue;
                        if (_validationSeverity == ValidationSeverity.Strict)
                            throw new ArgumentException("Zero is not allowed.");
                        return 1;
                    }
                default:
                    return propertyValue;
            }
        }

        /// <summary>
        /// Apply the current constraint to the decimal property.
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private decimal ApplyDecimalConstraint(decimal propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;
            switch (constraint.ConstraintType)
            {
                case ConstraintType.MaxDecimalPlaces:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return decimal.Round(propertyValue, constraintRefValue.Value, MidpointRounding.ToZero);
                    }
                case ConstraintType.RoundTo:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return decimal.Round(propertyValue, constraintRefValue.Value, MidpointRounding.AwayFromZero);
                    }

                case ConstraintType.Positive:
                    return propertyValue < 0m ? 0m : propertyValue;

                case ConstraintType.StrictPositive:
                    return propertyValue > 0m ? propertyValue : 1m;

                case ConstraintType.NonZero:
                    if (propertyValue != 0m) return propertyValue;
                    if (_validationSeverity == ValidationSeverity.Strict)
                        throw new ArgumentException("Zero is not allowed.");
                    return 1m;

                default:
                    return propertyValue;
            }
        }

        /// <summary>
        /// Apply the current constraint to the double property.
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private double ApplyDoubleConstraint(double propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;
            switch (constraint.ConstraintType)
            {
                case ConstraintType.MaxDecimalPlaces:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        double step = Math.Pow(10, constraintRefValue.Value);
                        return Math.Truncate(propertyValue * step) / step;
                    }

                case ConstraintType.RoundTo:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");

                        }

                        return Math.Round(propertyValue, constraintRefValue.Value, MidpointRounding.AwayFromZero);
                    }

                case ConstraintType.Positive:
                    return propertyValue < 0d ? 0d : propertyValue;

                case ConstraintType.StrictPositive:
                    return propertyValue > 0d ? propertyValue : 1d;

                case ConstraintType.NonZero:
                    if (propertyValue != 0d) return propertyValue;
                    if (_validationSeverity == ValidationSeverity.Strict)
                        throw new ArgumentException("Zero is not allowed.");
                    return 1d;

                default:
                    return propertyValue;
            }
        }

        /// <summary>
        /// Applies the specified constraint to a signed 64-bit integer property.
        /// </summary>
        /// <param name="propertyValue">The original <see cref="long"/> property value.</param>
        /// <param name="constraint">The <see cref="Constraint"/> to apply.</param>
        /// <returns>The sanitized <see cref="long"/> value according to the constraint.</returns>
        private long ApplyInt64Constraint(long propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;

            switch (constraint.ConstraintType)
            {
                case ConstraintType.MinValue:
                    if (constraintRefValue == null) throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                    return Math.Max(propertyValue, (long)constraintRefValue.Value);

                case ConstraintType.MaxValue:
                    if (constraintRefValue == null) throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                    return Math.Min(propertyValue, (long)constraintRefValue.Value);

                case ConstraintType.NotNegative:
                    return Math.Max(propertyValue, 0L);

                case ConstraintType.StrictPositive:
                    return propertyValue > 0L ? propertyValue : 1L;

                case ConstraintType.NonZero:
                    if (propertyValue != 0L) return propertyValue;
                    if (_validationSeverity == ValidationSeverity.Strict)
                        throw new ArgumentException("Zero is not allowed.");
                    return 1L;

                default:
                    return propertyValue;
            }
        }

        /// <summary>
        /// Applies the specified constraint to an unsigned 64-bit integer property.
        /// </summary>
        /// <param name="propertyValue">The original <see cref="ulong"/> property value.</param>
        /// <param name="constraint">The <see cref="Constraint"/> to apply.</param>
        /// <returns>The sanitized <see cref="ulong"/> value according to the constraint.</returns>
        private ulong ApplyUInt64Constraint(ulong propertyValue, Constraint constraint)
        {
            var constraintRefValue = constraint.ConstraintValue?.IntegerValue;

            switch (constraint.ConstraintType)
            {
                case ConstraintType.MinValue:
                    if (constraintRefValue == null) throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                    var minU = (ulong)Math.Max(0, constraintRefValue.Value);
                    return propertyValue < minU ? minU : propertyValue;

                case ConstraintType.MaxValue:
                    if (constraintRefValue == null) throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                    var maxU = (ulong)Math.Max(0, constraintRefValue.Value);
                    return propertyValue > maxU ? maxU : propertyValue;

                case ConstraintType.NotNegative:
                case ConstraintType.Positive:
                    return propertyValue;

                case ConstraintType.StrictPositive:
                    return propertyValue == 0UL ? 1UL : propertyValue;

                case ConstraintType.NonZero:
                    if (propertyValue != 0UL) return propertyValue;
                    if (_validationSeverity == ValidationSeverity.Strict)
                        throw new ArgumentException("Zero is not allowed.");
                    return 1UL;

                default:
                    return propertyValue;
            }
        }

        /// <summary>
        /// Apply the current constraint to the object property.
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private object ApplyObjectConstraint(object propertyValue, Type propertyType, Constraint constraint)
        {
            switch (constraint.ConstraintType)
            {
                case ConstraintType.NotNull:
                    {
                        if(propertyValue == null)
                            return Activator.CreateInstance(propertyType);
                        return propertyValue;
                    }
                case ConstraintType.NoDbNull:
                    {
                        if(propertyValue == DBNull.Value)
                            return null;
                        return propertyValue;
                    }
                case ConstraintType.NotEmptyCollection:
                    {
                        if (propertyValue == null)
                        {
                            if (_validationSeverity == ValidationSeverity.Strict)
                                throw new ArgumentException("Collection is null.");
                            return propertyValue;
                        }

                        if (propertyValue is string) return propertyValue;

                        if (propertyValue is System.Collections.IEnumerable enumerable)
                        {
                            var hasAny = enumerable.GetEnumerator().MoveNext();
                            if (!hasAny)
                            {
                                if (_validationSeverity == ValidationSeverity.Strict)
                                    throw new ArgumentException("Collection is empty.");
                            }
                        }
                        return propertyValue;
                    }
                case ConstraintType.DistinctCollection:
                    {
                        if (propertyValue == null) return null;
                        if (propertyValue is string) return propertyValue;

                        if (propertyValue is System.Collections.IEnumerable enumerable)
                        {
                            var seen = new HashSet<object>();
                            var distinct = new System.Collections.ArrayList();
                            foreach (var item in enumerable)
                            {
                                if (seen.Add(item))
                                    distinct.Add(item);
                            }

                            if (propertyValue is System.Collections.IList list && !list.IsReadOnly && !list.IsFixedSize)
                            {
                                list.Clear();
                                foreach (var item in distinct) list.Add(item);
                                return list;
                            }

                            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                try
                                {
                                    var elemType = propertyType.GetGenericArguments()[0];
                                    var typedList = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
                                    foreach (var item in distinct)
                                        typedList.Add(item);
                                    return typedList;
                                }
                                catch
                                {
                                    // fallback
                                }
                            }

                            return propertyValue;
                        }

                        return propertyValue;
                    }
                default:
                    return propertyValue;
            }
        }
    }
}
