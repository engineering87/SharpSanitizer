# SharpSanitizer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/SharpSanitizer?style=plastic)](https://www.nuget.org/packages/SharpSanitizer)
![NuGet Downloads](https://img.shields.io/nuget/dt/SharpSanitizer)
[![issues - SharpSanitizer](https://img.shields.io/github/issues/engineering87/SharpSanitizer)](https://github.com/engineering87/SharpSanitizer/issues)
[![Build](https://github.com/engineering87/SharpSanitizer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/SharpSanitizer/actions/workflows/dotnet.yml)
[![stars - SharpSanitizer](https://img.shields.io/github/stars/engineering87/SharpSanitizer?style=social)](https://github.com/engineering87/SharpSanitizer)

SharpSanitizer is a .NET library that allows to sanitize the properties of a generic object by specifying rules and constraints for the individual properties.
The constraints and contextual sanitization rules are specified parametrically.

### How it works
SharpSanitizer proceeds to evaluate each property of the object to be sanitized, checking the presence of rules and constraints specific to the individual properties. Depending on the type of data, it applies a cleaning rule to the final value.

### How to use it
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

### Supported rules

Currently the supported rules are as follows:

```csharp
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
        ForceValidGuid
```

rules and data types will be extended in future versions.

### NuGet

The library is available on NuGet packetmanager.

https://www.nuget.org/packages/SharpSanitizer/

## Contributing

Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/SharpSanitizer/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
SharpSanitizer source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
