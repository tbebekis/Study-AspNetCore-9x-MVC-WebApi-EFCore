namespace MvcApp.Library
{
    /// <summary>
    /// HSTS settings.
    /// <para>
    /// HTTP Strict Transport Security (HSTS) is a web security policy mechanism 
    /// that helps to protect websites against protocol downgrade attacks[1] and cookie hijacking. 
    /// </para>
    /// It allows web servers to declare that web browsers (or other complying user agents) should interact with it 
    /// using only secure HTTPS connections,[2] and never via the insecure HTTP protocol. 
    /// <para>
    /// A server implements an HSTS policy by supplying a header over an HTTPS connection (HSTS headers over HTTP are ignored).
    /// For example, a server could send a header such that future requests to the domain for the next year 
    /// (max-age is specified in seconds; 31,536,000 is equal to one non-leap year) use only HTTPS: Strict-Transport-Security: max-age=31536000
    /// </para>
    /// <para>The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.</para>
    /// <para></para>
    /// <para>SEE: https://en.wikipedia.org/wiki/HTTP_Strict_Transport_Security </para>
    /// <para>SEE: https://aka.ms/aspnetcore-hsts </para>
    /// <para>SEE: https://hstspreload.org/ </para>
    /// </summary>
    public class HSTSSettings
    {
        /// <summary>
        /// How many ours to apply the policy
        /// </summary>
        public int MaxAgeHours { get; set; }
        /// <summary>
        /// Preload. See https://hstspreload.org/
        /// </summary>
        public bool Preload { get; set; }
        /// <summary>
        /// When true applies the HSTS policy to Host subdomains
        /// </summary>
        public bool IncludeSubDomains { get; set; }
        /// <summary>
        /// A list of hosts to exclude
        /// </summary>
        public List<string> ExcludedHosts { get; set; } = new List<string>();
    }
}
