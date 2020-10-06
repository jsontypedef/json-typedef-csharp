# Jtd.Jtd: JSON Validation for CSharp / .NET

[![Nuget](https://img.shields.io/nuget/v/Jtd.Jtd)](https://www.nuget.org/packages/Jtd.Jtd)

`Jtd.Jtd` is a CSharp implementation of [JSON Type Definition][jtd], a
standardized schema language for JSON. `Jtd.Jtd` primarily gives you two things:

1. You can validate input data against JSON Typedef schemas. With this package,
   you can add JSON Typedef-powered validation to your application.

2. You get a CSharp representation of JSON Typedef schemas. That means you can
   use `Jtd.Jtd` to build your own tooling on top of JSON Type Definition.

This package works with both [`Newtonsoft.Json` (aka: "Json.NET")][newtonsoft]
and [`System.Text.Json`][system-text-json].

## Installation

You can install `Jtd.Jtd` by with `dotnet`:

```bash
dotnet add package Jtd.Jtd
```

For alternative installation instrutions, check out the package on Nuget:

https://www.nuget.org/packages/Jtd.Jtd

## Documentation

Detailed API documentation is available online at:

https://jsontypedef.github.io/json-typedef-csharp/api/Jtd.Jtd.html

For more high-level documentation about JSON Typedef in general, or JSON Typedef
in combination with CSharp in particular, check out:

* [The JSON Typedef Website][jtd]

## Usage

### With `Newtonsoft.Json`

```csharp
using System;
using System.Collections.Generic;
using Jtd.Jtd;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// The Schema class is a POCO which you can construct yourself if
// you like, but in this example we'll construct it from JSON.
Schema schema = JsonConvert.DeserializeObject<Schema>(@"
    {
        ""properties"": {
            ""name"": { ""type"": ""string"" },
            ""age"": { ""type"": ""uint8"" },
            ""phones"": {
                ""elements"": { ""type"": ""string"" }
            }
        }
    }
");

// Validators can take a schema and an input, and find validation
// errors in the input.
//
// Validators are backend-netural; they support both Newtonsoft.Json
// and System.Text.Json. To make that work, you'll have to use
// NewtonsoftAdapter or SystemTextAdapter depending on what you're
// using. Or you could implement Jtd's IJson interface yourself.
Validator validator = new Validator();

// Validator.Validate(Schema, IJson) returns an array of validation
// errors. If there were no problems with the input, it returns an
// empty array.
//
// This input is perfect, so we'll get back an empty list of
// validation errors.
string okJson = "{\"name\":\"John Doe\",\"age\":43,\"phones\":[\"+44 1234567\",\"+44 2345678\"]}";
IList<ValidationError> okErrors = validator.Validate(schema,
    new NewtonsoftAdapter(JToken.Parse(okJson)));

// Outputs: 0
Console.WriteLine(okErrors.Count);

// This next input has three problems with it:
//
// 1. It's missing "name", which is a required property.
// 2. "age" is a string, but it should be an integer.
// 3. "phones[1]" is a number, but it should be a string.
//
// Each of those errors corresponds to one of the errors returned by
// Validator.validate().
string badJson = "{ \"age\": \"43\", \"phones\": [\"+44 1234567\", 442345678] }";
IList<ValidationError> badErrors = validator.Validate(schema,
    new NewtonsoftAdapter(JToken.Parse(badJson)));

// Outputs: 3
Console.WriteLine(badErrors.Count);

// Outputs: error at: , due to: properties/name
//
// This means that "name" is missing.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[0].InstancePath),
    string.Join("/", badErrors[0].SchemaPath));

// Outputs: error at: age, due to: properties/age/type
//
// This means that "age" has the wrong type.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[1].InstancePath),
    string.Join("/", badErrors[1].SchemaPath));

// error at: phones/1, due to: properties/phones/elements/type
//
// This means that "phones[1]" has the wrong type.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[2].InstancePath),
    string.Join("/", badErrors[2].SchemaPath));
```

### With `System.Text.Json`

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using Jtd.Jtd;

// The Schema class is a POCO which you can construct yourself if
// you like, but in this example we'll construct it from JSON.
Schema schema = JsonSerializer.Deserialize<Schema>(@"
    {
        ""properties"": {
            ""name"": { ""type"": ""string"" },
            ""age"": { ""type"": ""uint8"" },
            ""phones"": {
                ""elements"": { ""type"": ""string"" }
            }
        }
    }
");

// Validators can take a schema and an input, and find validation
// errors in the input.
//
// Validators are backend-netural; they support both Newtonsoft.Json
// and System.Text.Json. To make that work, you'll have to use
// NewtonsoftAdapter or SystemTextAdapter depending on what you're
// using. Or you could implement Jtd's IJson interface yourself.
Validator validator = new Validator();

// Validator.Validate(Schema, IJson) returns an array of validation
// errors. If there were no problems with the input, it returns an
// empty array.
//
// This input is perfect, so we'll get back an empty list of
// validation errors.
string okJson = "{\"name\":\"John Doe\",\"age\":43,\"phones\":[\"+44 1234567\",\"+44 2345678\"]}";
IList<ValidationError> okErrors = validator.Validate(schema,
    new SystemTextAdapter(JsonDocument.Parse(okJson).RootElement));

// Outputs: 0
Console.WriteLine(okErrors.Count);

// This next input has three problems with it:
//
// 1. It's missing "name", which is a required property.
// 2. "age" is a string, but it should be an integer.
// 3. "phones[1]" is a number, but it should be a string.
//
// Each of those errors corresponds to one of the errors returned by
// Validator.validate().
string badJson = "{ \"age\": \"43\", \"phones\": [\"+44 1234567\", 442345678] }";
IList<ValidationError> badErrors = validator.Validate(schema,
    new SystemTextAdapter(JsonDocument.Parse(badJson).RootElement));

// Outputs: 3
Console.WriteLine(badErrors.Count);

// Outputs: error at: , due to: properties/name
//
// This means that "name" is missing.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[0].InstancePath),
    string.Join("/", badErrors[0].SchemaPath));

// Outputs: error at: age, due to: properties/age/type
//
// This means that "age" has the wrong type.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[1].InstancePath),
    string.Join("/", badErrors[1].SchemaPath));

// error at: phones/1, due to: properties/phones/elements/type
//
// This means that "phones[1]" has the wrong type.
Console.WriteLine("error at: {0}, due to: {1}",
    string.Join("/", badErrors[2].InstancePath),
    string.Join("/", badErrors[2].SchemaPath));
```

## Advanced Usage

### Limiting Erros Returned

By default, `Validator.Validate(Schema, IJson)` returns every error it finds. If
you just care about whether there are any errors at all, or if you can't show
more than some number of errors, then you can get better performance out of
`Validate` using the `MaxErrors` attribute on `Validator`.

For example, taking the same example from before, but limiting it to 1 error, we
get:

```csharp
// Instead of doing new Validator(), we do:
Validator validator = new Validator { MaxErrors = 1 };

// Previously, this would return three errors. Now it returns just one.
IList<ValidationError> badErrors = validator.Validate(schema,
    new SystemTextAdapter(JsonDocument.Parse(badJson).RootElement));

// Outputs: 1
Console.WriteLine(badErrors.Count);
```

### Handling Untrusted Schemas

If you want to run `Jtd.Jtd` against a schema that you don't trust, then you
should:

1. Make sure the schema is well-formed, using the `Verify` method on `Schema`.
   That will check things like making sure every `ref` has a corresponding
   definition.

2. Set the `MaxDepth` attribute on `Validator`. JSON Typedef lets you write
   recursive schemas, which means if you're evaluating against untrusted
   schemas, you might go into an infinite loop when dealing with a malicious
   input like:

   ```json
   {
       "ref": "loop",
       "definitions": {
           "loop": {
               "ref": "loop"
           }
       }
   }
   ```

   The `MaxDepth` attribute tells `Validator.Validate(Schema, IJson)` how many
   `ref`s to follow before giving up and throwing `MaxDepthExceededException`.
   You can then catch and handle that exception, rather than going into a stack
   overflow.

Here's an example of how you can use `Jtd.Jtd` to evaluate data against an
untrusted schema:

```csharp
/// <summary>
/// Validates <paramref name="instance" /> against <paramref
/// name="schema" />, guarding against the possibility that <paramref
/// name="schema" /> has circular definitions or is invalid.
/// </summary>
///
/// <exception cref="MaxDepthExceededException">
/// When <paramref name="schema" /> has a circular reference loop.
/// </exception>
///
/// <exception cref="InvalidSchemaException">
/// When <paramref name="schema" /> isn't a valid schema.
/// </exception>
///
/// <returns>
/// Whether the input is valid against the schema.
/// </returns>
public static bool ValidateUntrusted(Schema schema, IJson instance)
{
    // Throws InvalidSchemaException if schema isn't valid.
    schema.Verify();

    // You should tune MaxDepth to be high enough that most legitimate
    // schemas evaluate without errors, but low enough that an attacker
    // cannot cause a denial of service attack.
    Validator validator = new Validator { MaxDepth = 32 };
    return validator.Validate(schema, instance).Count == 0;
}

public static void Main(string[] args)
{
    // Returns: true
    ValidateUntrusted(
        new Schema { Type = Jtd.Jtd.Type.String },
        new NewtonsoftAdapter(JToken.Parse("\"foo\"")));

    // Returns: false
    ValidateUntrusted(
        new Schema { Type = Jtd.Jtd.Type.String },
        new NewtonsoftAdapter(JToken.Parse("null")));

    // Raises:
    //
    // Jtd.Jtd.InvalidSchemaException: ref to non-existent definition
    ValidateUntrusted(
        new Schema { Ref = "this definition does not exit" },
        new NewtonsoftAdapter(JToken.Parse("null")));

    // Raises:
    //
    // Jtd.Jtd.MaxDepthExceededException: max depth exceeded during Validator.Validate
    ValidateUntrusted(
        new Schema {
            Ref = "loop",
            Definitions = new Dictionary<string, Schema>() {
                { "loop", new Schema { Ref = "loop" }}
            },
        },
        new NewtonsoftAdapter(JToken.Parse("null")));
}
```

[jtd]: https://jsontypedef.com/
[newtonsoft]: https://www.newtonsoft.com/json
[system-text-json]: https://docs.microsoft.com/en-us/dotnet/api/system.text.json
