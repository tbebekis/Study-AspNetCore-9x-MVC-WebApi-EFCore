namespace StudyLib
{
    static public class StringExtensions
    {
        /// <summary>
        /// Case insensitive string equality.
        /// <para>Returns true if 1. both are null, 2. both are empty string or 3. they are the same string </para>
        /// </summary>
        static public bool IsSameText(this string A, string B)
        {
            //return (!string.IsNullOrWhiteSpace(A) && !string.IsNullOrWhiteSpace(B))&& (string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0);

            // Compare() returns true if 1. both are null, 2. both are empty string or 3. they are the same string
            return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        /// <summary>
        /// Returns true if Value is contained in the Instance.
        /// Performs a case-insensitive check using the invariant culture.
        /// </summary>
        static public bool ContainsText(this string Instance, string Value)
        {
            if ((Instance != null) && !string.IsNullOrWhiteSpace(Value))
            {
                return Instance.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) != -1;
            }

            return false;
        }


        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        static public string HtmlEncode(this string Text)
        {

            return string.IsNullOrWhiteSpace(Text) ? Text : WebUtility.HtmlEncode(Text);
        }
        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string
        /// </summary>
        static public string HtmlDecode(this string Text)
        {
            return string.IsNullOrWhiteSpace(Text) ? Text : WebUtility.HtmlDecode(Text);
        }
        /// <summary>
        /// Converts a string to an Url-encoded string.
        /// </summary>
        static public string UrlEncode(this string Text)
        {
            return string.IsNullOrWhiteSpace(Text) ? Text : WebUtility.UrlEncode(Text);
        }
        /// <summary>
        /// Converts a string that has been Url-encoded into a decoded string
        /// </summary>
        static public string UrlDecode(this string Text)
        {
            return string.IsNullOrWhiteSpace(Text) ? Text : WebUtility.UrlDecode(Text);
        }

    }
}
