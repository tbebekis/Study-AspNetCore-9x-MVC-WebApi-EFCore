namespace StudyLib
{
    /// <summary>
    /// Handles decimals.
    /// <para>We can NOT have an action with a double or decimal parameter, e.g. GetCartTotals(string CustomerId, decimal ShippingCharge).</para>
    /// <para>The default ModelBinder converts 123.45 to 12345.</para>
    /// <para>The reason is that the default ModelBinder sees doubles/decimals as integers if they are NOT JSON.stringified(), as when we send a model. </para>
    /// <para>But in cases where there is no a model, but just simple value parameteres to an action, the convertion fails.</para>
    /// <para>So to handle cases like that this custom binder is used.</para>
    /// <para>SEE: https://stackoverflow.com/questions/32908503/c-sharp-mvc-controller-cannot-get-decimal-or-double-values-from-ajax-post-reques </para>
    /// <para>SEE: https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding </para>
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        static bool TryParseInvariant(string S, out decimal Value)
        {
            Value = 0;
            try
            {
                Value = decimal.Parse(S, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
            }

            return false;
        }
        /// <summary>
        /// Converts a string into a decimal. Returns true on success.
        /// </summary>
        static bool TryParse(string S, out decimal Value)
        {
            Value = 0;
            bool Result = TryParseInvariant(S, out Value);
            if (!Result)
                Result = decimal.TryParse(S, out Value);

            return Result;
        }

        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                string value = valueProviderResult.FirstValue;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    decimal Result = 0;
                    if (TryParse(value, out Result))
                    {
                        bindingContext.Result = ModelBindingResult.Success(Result);
                    }
                    else
                    {
                        bindingContext.ModelState.TryAddModelError(modelName, $"Cannot convert {value} to decimal");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
