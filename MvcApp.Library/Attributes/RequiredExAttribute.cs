namespace MvcApp.Library
{
    /// <summary>
    /// Extension for the Required attribute
    /// </summary>
    public class RequiredExAttribute : RequiredAttribute
    {
        /// <summary>
        /// Applies formatting to an error message based on the data field where the error occurred.
        /// </summary>
        public override string FormatErrorMessage(string name)
        {
            string Default = "Field {0} is required";
            string Format = Lib.Localize("RequiredField", Default);
            return string.Format(Format, name);
        }
    }

 
}
