# Model Validation

> This text is part of a group of texts describing [Asp.Net Core](Index.md).

[Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation) is term describing a procedure where model properties are required to conform to a set of rules in order for the model to be considered valid.

When the model validation fails then proper error messages are displayed to the user of an MVC application or are returned to the client of a WebApi application.

The error messages should provide enough information for the user or client to deal with the problems.

## The ModelState Dictionary

In Asp.Net Core model validation happens in controller action methods automatically.

Controllers of MVC or WebApi applications inherit from [ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase) controller.

`ControllerBase` controller provides the [ModelState](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.modelstate) property, of type [ModelStateDictionary](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelstatedictionary).

`ModelState` property automatically contains model validation information. 

`ModelState` dictionary is a collection of `Key-Value` pairs where `Key` is the name of a model property and `Value` is a [ModelStateEntry](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelstateentry) instance.

The `ModelStateEntry.ValidationState` property, of the enum type [ModelValidationState](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.modelvalidationstate), indicates whether a **model property** is valid or not.

```
IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidEntries = Controller.ModelState.Where(entry => entry.Value.ValidationState == ModelValidationState.Invalid).Select(entry => entry);
```

For the overall model validity the `ModelState.IsValid` property is used.

```
if (ModelState.IsValid)
{
    // model is valid 
}
else
{
    // model is not valid 
}
```

## MVC Application - Standard Handling of Model Validation Errors

```
[ValidateAntiForgeryToken]
[HttpPost(Name = "Product.Save")]
public async Task<ActionResult> Save(ProductModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // model is valid, handle the request
    // ...

    return View(model);
}
```

In a MVC application the typical handling of model validation errors is to return the view again to the user.

## WebApi Application - Standard Handling of Model Validation Errors
### WebApi

```
[HttpPost()]
public async Task<ActionResult> Save(ProductModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest();
    }

    // model is valid, handle the request
    // ...

    return Model;
}
```

In a WebApi application the typical handling of model validation errors is to return a `BadRequest()` result.

> Actually [a WebApi application](https://learn.microsoft.com/en-us/aspnet/core/web-api/#automatic-http-400-responses), with the default configuration, has no need to return a HTTP 400 error with a `BadRequest()` result. In case of an invalid model, and before the request even reaches the controller, Asp.Net Core framework sends a `BadRequest()` response.

## Clear or Add Model Validation Errors

The `ClearValidationState()` clears any errors in the `ModelState` dictionary.

```
ModelState.ClearValidationState(nameof(MyModel));
```

The `ModelState.AddModelError(PropertyName, ErrorMessage)` used in custom validation to add errors in the `ModelState` dictionary.

```
ModelState.AddModelError(nameof(MyModel.MyProperty), "MyProperty has not a valid value");
```

The `ControllerBase.TryValidateModel()` can be used to validate or re-validate the model.

```
if (!TryValidateModel(Movie))
{
    return View();
}
```

## Model Validation Attributes

One way to specify model validation rules, in model properties, is to use model validation attributes.

Model validation attributes ultimately inherit from [ValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute).

The `ValidationAttribute.ErrorMessage` property is used, along with the validation rule, to display a descriptive error message.

```
[Range(0, 1000, ErrorMessage = "Price valid range is >= 0 and <= 1000")]
public decimal Price { get; set; }
```

[System.ComponentModel.DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) namespace is where most of the validation attributes are defined.

The list goes in a great length.

- [AllowedValuesAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.allowedvaluesattribute). Specifies a list of allowed values.
- [Base64StringAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.base64stringattribute). Specifies that the value should be a valid Base64 string.
- [CompareAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.compareattribute): Specifies that two properties should match.
- [CreditCardAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations). Specifies that a d value should be a credit card number. Requires JQuery validation.
- [CustomValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.customvalidationattribute). Specifies a custom validation method that is used to validate a model or a property.
- [DeniedValuesAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.deniedvaluesattribute). Specifies a list of **not** allowed values.
- [EmailAddressAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute). Specifies that the value should be a valid email.
- [LengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.lengthattribute). Used with collection or string properties. Specifies the minimum and maximum length allowed.
- [MinLengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.minlengthattribute). Used with array or string properties. Specifies the minimum length allowed.
- [MaxLengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.maxlengthattribute). Used with array or string properties. Specifies the maximum length allowed.
- [StringLengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute). Specifies the maximum length allowed of a string property.
- [PhoneAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.phoneattribute). Specifies that the value should be a valid telephone format.
- [RangeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute). Specifies the valid range of a value.
- [RegularExpressionAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.regularexpressionattribute). Specifies that the value should match a specified regular expression.
- [RemoteAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.remoteattribute). Validates input on the client by calling an action method on the server. 
- [RequiredAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute). Specifies that the value cannot be null. Nullable and non-nullable values are [handled differently](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation#non-nullable-reference-types-and-required-attribute).
- [UrlAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.urlattribute). Specifies that the value should have a URL format.
- [ValidateNeverAttribute](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.modelbinding.validation.validateneverattribute). Specifies that a property or parameter should be excluded from validation.


## Custom Model Validation

Custom model validation can be done in a number of ways. Some of them are discussed next.

### Custom Model Validation - Using a ValidationAttribute subclass

Subclassing [ValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute) a developer can create specialized  validation attributes for model properties.

```
public class Person
{
    [MinBirthYear(Year = 1930)]
    public DateTime DateOfBirth { get; set; }  
}

[AttributeUsage(AttributeTargets.Property)]
public class MinBirthYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null && value.GetType() == typeof(DateTime))
        {
            DateTime DateOfBirth = (DateTime)value;

            int BirthYear = DateOfBirth.Year;
            if (BirthYear < Year)
                return new ValidationResult($"Valid minimum birth year is {Year}.");                
        }

        return ValidationResult.Success;
    }

    public int Year { get; set; }
}
```

The [ValidationContext](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationcontext) parameter contains a number of very useful properties and methods. 

### Custom Model Validation - Using the built-in CustomValidationAttribute

[CustomValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.customvalidationattribute) can annotate a model class or a model property.

`CustomValidationAttribute` is a kind of a peculiar model validation attribute. It is not meant to be inherited. 

`CustomValidationAttribute` requires a validator type and the name of a validation method belonging to that validator type. 

The validation method should be a static public method and can have any of the two following signatures.

```
static public ValidationResult AnyMethodName(AnyType AnyValue);
static public ValidationResult AnyMethodName(AnyType AnyValue, ValidationContext context);
```

The first parameter of the validation method can be of a strongly type.

An example of use.

```
public class Person
{
    [CustomValidation(typeof(DateOfBirthValidator), "IsValid")]
    public DateTime DateOfBirth { get; set; }  
}

static public class DateOfBirthValidator
{
    static public ValidationResult IsValid(DateTime DateOfBirth, ValidationContext context)
    {
        // get the model instance
        // it may be used in validating multiple properties
        Person person = context.ObjectInstance as Person;

        int Year = 1930;
        int BirthYear = DateOfBirth.Year;
        if (BirthYear < Year)
            return new ValidationResult($"Valid minimum birth year is {Year}.");

        return ValidationResult.Success;
    }
}
```

### Custom Model Validation - Using an IValidatableObject implementation

The [IValidatableObject](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation#ivalidatableobject) is meant to be implemented by model classes, thus making a model class `self-validatable`.

```
public class Person: IValidatableObject
{
    public DateTime DateOfBirth { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        int Year = 1930;
        int BirthYear = DateOfBirth.Year;
        if (BirthYear < Year)
            yield  return new ValidationResult($"Valid minimum birth year is {Year}.");

        yield  return ValidationResult.Success;
    }
}
```

 

### TODO

ValidateAntiForgeryToken

asp-validation-summary

IValidatableObject
Remote Validation
Global Model Validation