# SharpSanitizer
SharpSanitizer is a C# .NET 5.0 library that allows to sanitize the properties of a generic object by specifying rules and constraints for the individual properties.
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

at this point it is enough to invoke the method **Sanitize** to apply the rules to the object:

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
        NoSpecialCharacters
```
rules and data types will be extended in future versions.

### NuGet

The library is available on NuGet packetmanager.

https://www.nuget.org/packages/SharpSanitizer/

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/SharpSanitizer/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
SharpSanitizer source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
