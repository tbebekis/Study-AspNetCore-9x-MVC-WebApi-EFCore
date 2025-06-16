namespace MvcApp.Library
{
 
    /// <summary>
    /// A metadata provider that adds custom attributes to a model's metadata.
    /// <para>Those attributes can be retrieved later. For an example see <see cref="TagHelperControlRow"/> </para>
    /// <para>For the retrieval to work, this provider should be registered in <c>ConfigureServices()</c> as </para>
    /// <para> <c>MvcBuilder.AddMvcOptions(o => o.ModelMetadataDetailsProviders.Add(new ModelMetadataProvider()));</c> </para>
    /// <para>To retrieve the attribute from inside a TagHelper use the following  </para>
    /// <para><c> if (For.Metadata.AdditionalValues.TryGetValue(nameof(MyCustomAttribute), out object value)) {...}</c></para>
    /// <para>See Also: https://github.com/aspnet/Mvc/issues/4597 </para>
    /// </summary>
    public class TagHelperModelMetadataProvider : IDisplayMetadataProvider //, IValidationMetadataProvider
    {
        /// <summary>
        /// Sets the additional values for a <see cref="DisplayMetadataProviderContext.DisplayMetadata"/>  
        /// </summary>
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            // get all custom attributes of the model
            List<IModelAttribute> CustomAttributeList = context.Attributes.OfType<IModelAttribute>().ToList();

            // add all custom attributes of the model as additional metadata values 
            foreach (var A in CustomAttributeList)
            {
                if (context.DisplayMetadata.AdditionalValues.ContainsKey(A.ClassName))
                    throw new ApplicationException($"There is already an attribute with the same name on this model: {A.ClassName}");

                context.DisplayMetadata.AdditionalValues.Add(A.ClassName, A);
            }
        }
    }
}
