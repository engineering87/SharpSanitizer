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
                var constraintFound = _propertiesConstraints.TryGetValue(pi.Name, out Constraint constraint);
                if (!constraintFound) continue;

                var propertyValue = pi.GetValue(obj, null);
                var propertyType = Type.GetTypeCode(pi.PropertyType);
                switch (propertyType)
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
                        break;
                    case TypeCode.Byte:
                        break;
                    case TypeCode.Int16:
                        {
                            var propertyValueSanitize = ApplyIntegerConstraint((int)propertyValue, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.UInt16:
                        break;
                    case TypeCode.Int32:
                        {
                            var propertyValueSanitize = ApplyIntegerConstraint((int)propertyValue, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.UInt32:
                        break;
                    case TypeCode.Int64:
                        break;
                    case TypeCode.UInt64:
                        break;
                    case TypeCode.Single:
                        break;
                    case TypeCode.Double:
                        {
                            var propertyValueSanitize = ApplyDoubleConstraint((double)propertyValue, constraint);
                            pi.SetValue(obj, propertyValueSanitize);
                            break;
                        }
                    case TypeCode.Decimal:
                        {
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
                        return trimString?.Length <= constraintRefValue.Value
                            ? trimString : trimString?.PadRight(constraintRefValue.Value);
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
                        return propertyValue.Trim().Substring(0, 1);
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

                        return propertyValue > constraintRefValue.Value ? constraintRefValue.Value : propertyValue;
                    }
                case ConstraintType.MaxValue:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return propertyValue > constraintRefValue.Value ? constraintRefValue.Value : propertyValue;
                    }
                case ConstraintType.NotNegative:
                    {
                        if (constraintRefValue == null)
                        {
                            throw new ArgumentNullException("The constraint value is NULL or not set for the specified ConstraintType");
                        }

                        return propertyValue < 0 ? constraintRefValue.Value : propertyValue;
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

                        decimal step = (decimal)Math.Pow(10, constraintRefValue.Value);
                        decimal tmp = Math.Truncate(step * propertyValue);
                        return tmp / step;
                    }
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
                default:
                    return propertyValue;
            }
        }
    }
}
