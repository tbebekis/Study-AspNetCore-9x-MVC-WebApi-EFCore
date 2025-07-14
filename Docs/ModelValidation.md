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

> Actually, with the default configuration, a controller action in a [a WebApi application](https://learn.microsoft.com/en-us/aspnet/core/web-api/#automatic-http-400-responses) has no need to return a HTTP 400 error with a `BadRequest()` result. In case of an invalid model, and before the request even reaches the controller action, Asp.Net Core framework sends a `BadRequest()` response to the client.

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

One way to specify model validation rules, in model properties, is to use model built-in validation attributes.

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
- [DataTypeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.datatypeattribute). Specifies the name of a [data type](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.datatype) to associate with a model property.
- [DeniedValuesAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.deniedvaluesattribute). Specifies a list of **not** allowed values.
- [EmailAddressAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute). Specifies that the value should be a valid email.
- [EnumDataTypeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.enumdatatypeattribute). Specifies a .NET enumeration type to be mapped to a model property.
- [FileExtensionsAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.fileextensionsattribute). Specifies valid file name extensions.
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

The built-in [CustomValidationAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.customvalidationattribute) can annotate a model class or a model property.

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

## Client-side Validation

Asp.Net Core supports client-side validation too, i.e. validation performed in the browser using `Javascript`.

Client-side validation prevents the `POST` of a `HTML Form` until all form's input elements contain valid values. Clicking on the form's `submit` button executes the validation `Javascripts` and if any of the form's input elements has an invalid value, then the form is not `POST-ed` and instead some descriptive error messages are displayed. 

Thus when there are input errors on a form, using client-side validation unnecessary round trips to the server are avoided.

The officila documentation sometimes refers to client-side validation as [unobtrusive validation](https://github.com/aspnet/jquery-validation-unobtrusive).

`Javascript` files required for client-side validation are the following.

- jquery.js
- jquery.validate.js
- jquery.validate.unobtrusive.js

An application can include these files using a [CDN](https://en.wikipedia.org/wiki/Content_delivery_network).

```
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.js"></script>
```

Asp.Net Core MVC template creates the `wwwroot\lib` containining local versions of these files instead of a CDN.

```
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js"></script>
```

Suppose the following model

```
public class ProductModel
{
    public ProductModel()
    {
    }

    public string Id { get; set; }
    [StringLength(100, MinimumLength = 5)]
    public string Name { get; set; }
    [Range(1, 100)]
    public double Price { get; set; }
}
```

the next action,

```
[HttpPost("edit", Name = "Product.Edit")]
[ValidateAntiForgeryToken]
public ActionResult Edit(ProductModel Model)
{
    // ...
}
```

and the following view.

```
@model ProductModel

<div>
    <h2>Product Edit</h2>
 
    <form asp-route="Product.Edit" method="post">
        <input asp-for="Id" type="hidden" />
        <label>Name: <input asp-for="Name" /></label> <br />
        <span asp-validation-for="Name" class="text-danger"></span> <br />
        <label>Price: <input asp-for="Price" /></label> <br />
        <span asp-validation-for="Price" class="text-danger"></span> <br />
        <input type="submit" value="Submit">
    </form>

</div>
```

Please notice the `asp-validation-for` tag helpers. 

The `asp-validation-for` tag helper adds a hidden span, associated to a property, with an error message which is displayed to the user if the property validation fails.

This is the HTML code produced by [razor](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor), just edited it to make it more readable.

```
<div>
    <h2>Product Edit</h2>
 
    <form method="post" action="/product/edit">
        <input type="hidden" id="Id" name="Id" value="34E78C4D-4170-4AE9-9DD7-2B129D21CC3C" />

        <label>Name: 
            <input type="text" 
            data-val="true" 
            data-val-length="The field Name must be a string with a minimum length of 5 and a maximum length of 100." 
            data-val-length-max="100" 
            data-val-length-min="5" 
            id="Name" 
            maxlength="100" 
            name="Name" 
            value="Potatoe" />
        </label> <br />

        <span class="text-danger field-validation-valid" data-valmsg-for="Name" data-valmsg-replace="true"></span> <br />
        
        <label>Price: 
            <input type="text" 
            data-val="true" 
            data-val-number="The field Price must be a number." 
            data-val-range="The field Price must be between 1 and 100." 
            data-val-range-max="100" 
            data-val-range-min="1" 
            data-val-required="The Price field is required." 
            id="Price" 
            name="Price" 
            value="1.5" />
        </label> <br />

        <span class="text-danger field-validation-valid" data-valmsg-for="Price" data-valmsg-replace="true"></span> <br />

        <input type="submit" value="Submit">

        <input name="__RequestVerificationToken" 
        type="hidden" value="CfDJ8L313PTzpvxLtek9kJucrJdYY2zIopIhwbvmEj8L3NYVFIodUGqb_LtMq2Yz7yp2f8P_YqQP8hziwfg4kRncAIUFDwbB-D60fxeFD6UxXyKwRyzOfVpteEOeKSv90dzCkf4JlwX3txqgY4OY284MGo92F6pZ-thIvYqVIHLdR4bEHVzjawiqhy8c0j_Psd0m1A" />
    </form>

</div>
```

- `data-val`. Specifies that the input requires validation.
- `data-val-required`. Specifies the error message to be displayed if a value is not provided.
- `data-val-length`, `data-val-number` and `data-val-range`. They are specific to the validation attribute applied to the property. They specify validation rules and error messages. 
- `data-valmsg-for`. Specifies the name, not the id, of the input associated to the error message.
- `data-valmsg-replace`. Specifies whether error messages can be replaced within this `span` element. The `asp-validation-for` tag helper always leaves this `span` empty.

> The `hidden input` with name `__RequestVerificationToken` is generated because of the `[ValidateAntiForgeryToken]` attribute applied to the controller action. That attribute is applied to `POST` controller actions in order to prevent [Cross-Site Request Forgery](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery) (XSRF/CSRF) attacks.

When the user clicks on the `Submit` button the `Javascript` validation code checks form's input elements and acts accordingly based on the above `data-*` attributes. If the validation fails then the form is not `POST-ed` to the server and an error message is displayed under every input element having an invalid value.

### Client-side Validation - Displaying error messages in one place

The [asp-validation-summary](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-validation-summary-tag-helper) can be used to display a summary of all validation error messages gathered in one place.

```
@model ProductModel

<div>
    <h2>Product Edit</h2>
 
    <form asp-route="Product.Edit" method="post">
        <div asp-validation-summary="All" class="text-danger"></div>
        <input asp-for="Id" type="hidden" />
        <label>Name: <input asp-for="Name" /></label> <br />
        <label>Price: <input asp-for="Price" /></label> <br />
        <span asp-validation-for="Price" class="text-danger"></span>
        <input type="submit" value="Submit">
    </form>

</div>
```

### Client-side Validation - Using the [Remote] attribute

Client-side [remote validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation#remote-attribute) involves the use of the [RemoteAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute).

The implementation of `remote validation` requires `Javascript` code to call a method on the server in order to determine whether a specified input element contains a valid value or not.


## Configure maximum validation errors and validation depth.

The developer can configure the maximum number of validation errors after which model validation stops.

Also because there maybe models that are deep or they can lead to infinite recursion which may result in stack overflow the maximum validation depth is configurable too. 

```
IMvcBuilder MvcBuilder = builder.Services.AddControllersWithViews();

MvcBuilder.AddMvcOptions(options => {
    options.MaxModelValidationErrors = 30;  // defaults to 200
    options.MaxValidationDepth = 16;        // defaults to 32
});
```
