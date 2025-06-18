namespace CommonLib
{
    /// <summary>
    /// Represents an object that provides resources such as string and images.
    /// <para>Each Assembly may contain a ResourceProvider class implementing this interface.</para>
    /// <para>It may contain more than one though, but <strong>the <see cref="Name"/> of the provider should be unique.</strong></para>
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Returns a resource string for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        string GetString(string Key, CultureInfo Culture = null);
        /// <summary>
        /// Returns a resource object for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        object GetObject(string Key, CultureInfo Culture = null);
        /// <summary>
        /// Returns a binary resource for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        byte[] GetBinary(string Key, CultureInfo Culture = null);
        /// <summary>
        /// Returns a resource Image for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// <para>NOTE: If in Windows, cast thre return object to the System.Drawing.Image class.</para>
        /// </summary>
        object GetImage(string Key, CultureInfo Culture = null);

        // ● properties
        /// <summary>
        /// The name of this provider. Must be unique.
        /// </summary>
        string Name { get; }

    }
}
