namespace CommonLib
{
    /// <summary>
    /// SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding
    /// </summary>
    public class ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(decimal))
                return new DecimalModelBinder();

            return null;
        }
    }
}
