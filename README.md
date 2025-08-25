# SharpSanitizer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/SharpSanitizer?style=plastic)](https://www.nuget.org/packages/SharpSanitizer)
![NuGet Downloads](https://img.shields.io/nuget/dt/SharpSanitizer)
[![issues - SharpSanitizer](https://img.shields.io/github/issues/engineering87/SharpSanitizer)](https://github.com/engineering87/SharpSanitizer/issues)
[![Build](https://github.com/engineering87/SharpSanitizer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/SharpSanitizer/actions/workflows/dotnet.yml)
[![stars - SharpSanitizer](https://img.shields.io/github/stars/engineering87/SharpSanitizer?style=social)](https://github.com/engineering87/SharpSanitizer)

**SharpSanitizer** is a lightweight and flexible .NET library designed to sanitize and validate object properties through a configurable set of rules and constraints.
It helps ensure data consistency, safety, and quality before persisting or processing objects.

## Features
- Strongly-typed, generic sanitizer with minimal boilerplate.
- Built-in constraints for strings, numbers, collections, and objects.
- Configurable via `Dictionary<string, Constraint>` for maximum flexibility.
- Works with both strict and relaxed validation modes.
- Support for advanced numeric rounding and collection-level checks.

## How it works
SharpSanitizer inspects the properties of an object and applies the configured constraints for each property.
The sanitization process modifies values in place based on the rules you define.

## Usage Example
To use the SharpSanitizer library, first populate the set of rules to be applied to the object properties. Rules are expressed through a *Dictionary* in which the key matches the **name** of the property and the value is the **constraint** to be applied, for example:

```csharp
var constraints = new Dictionary<string, Constraint>()
{
    { "StringMaxNotNull", new Constraint(ConstraintType.MaxNotNull, 10) },
    { "StringMax", new Constraint(ConstraintType.Max, 10) },
    { "StringNoWhiteSpace", new Constraint(ConstraintType.NoWhiteSpace) },
    { "StringNoSpecialCharacters", new Constraint(ConstraintType.NoSpecialCharacters) }
};
```
as a second step create an instance of the SharpSanitizer specifying the type of object to work on:

```csharp
var sharpSanitizer = new SharpSanitizer<FooModel>(constraints);
```

at this point it is enough to invoke the method `Sanitize` to apply the rules to the object:

```csharp
sharpSanitizer.Sanitize(fooModel);
```

### Supported Constraints

SharpSanitizer now supports a rich set of rules for validation and sanitization:

| Constraint              | Description                                                                 |
|--------------------------|-----------------------------------------------------------------------------|
| `NotNull`               | Ensures object is not null; initializes if needed.                         |
| `NoDbNull`              | Converts `DBNull.Value` to `null`.                                         |
| `NotNullOrEmpty`        | Ensures string is not null or empty.                                       |
| `NotNullOrWhiteSpace`   | Ensures string is not null or whitespace.                                  |
| `MaxLength`             | Truncates string to a maximum length.                                      |
| `MinLength`             | Pads string if shorter than required.                                      |
| `MaxNotNull`            | Non-null string limited to a maximum length.                              |
| `Uppercase`             | Converts string to uppercase.                                              |
| `Lowercase`             | Converts string to lowercase.                                              |
| `NoWhiteSpace`          | Removes all whitespace.                                                    |
| `NoSpecialCharacters`   | Removes all non-alphanumeric characters except `_` and `.`.                |
| `OnlyDigit`             | Keeps only digits in the string.                                           |
| `ValidDatetime`         | Keeps valid date strings, clears invalid.                                  |
| `ForceToValidDatetime`  | Forces invalid dates to `DateTime.MinValue`.                               |
| `SingleChar`            | Enforces single character.                                                 |
| `ValidGuid`             | Enforces a valid GUID (generates new if invalid in relaxed mode).          |
| `ValidEmail`            | Enforces a valid email format.                                             |
| `MaxValue`              | Numeric: clamps to max value.                                              |
| `MinValue`              | Numeric: clamps to min value.                                              |
| `NotNegative`           | Numeric: clamps negatives to zero.                                         |
| `Positive`              | Enforces value ≥ 0.                                                        |
| `StrictPositive`        | Enforces value > 0.                                                        |
| `NonZero`               | Disallows zero, adjusts based on severity mode.                            |
| `MaxDecimalPlaces`      | Truncates decimals to a maximum number of digits.                          |
| `RoundTo`               | Rounds decimals to a specified number of digits.                           |
| `NotEmptyCollection`    | Collection: must not be empty.                                             |
| `DistinctCollection`    | Collection: removes duplicates.  

## NuGet

The library is available on [NuGet](https://www.nuget.org/packages/SharpSanitizer/).

## Contributing

Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/SharpSanitizer/issues) if you encounter a bug or have a suggestion for improvements/features

## License
SharpSanitizer source code is available under MIT License, see license in the source.

## Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
