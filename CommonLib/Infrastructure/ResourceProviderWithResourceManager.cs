namespace CommonLib
{
    /// <summary>
    /// Represents an object that provides resources such as string and images.
    /// <para>Each Assembly may contain a ResourceProvider class implementing this interface.</para>
    /// <para>It may contain more than one though, but <strong>the <see cref="Name"/> of the provider should be unique.</strong></para>
    /// </summary>
    public class ResourceProviderWithResourceManager: ResourceProviderBase
    {
        ResourceManager Manager;

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceProviderWithResourceManager(ResourceManager Manager, string Name)
            : base(Name)
        {
            if (Manager == null)
                throw new ArgumentNullException("Manager");   

            this.Manager = Manager;  
        }

        // ● public
        /// <summary>
        /// Returns a resource string for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        public override string GetString(string Key, CultureInfo Culture = null)
        {
            return Manager.GetString(Key, Culture);
        }
        /// <summary>
        /// Returns a resource object for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        public override object GetObject(string Key, CultureInfo Culture = null)
        {
            return Manager.GetObject(Key, Culture);
        }
        /// <summary>
        /// Returns a binary resource for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// </summary>
        public override byte[] GetBinary(string Key, CultureInfo Culture = null)
        {
            return GetObject(Key, Culture) as byte[];
        }
        /// <summary>
        /// Returns a resource Image for the Key, if any, else null.
        /// <para>NOTE: Culture must be a specific culture (e.g. en-US, el-GR, etc.)</para>
        /// <para>NOTE: If not culture is specified the default culture is used.</para>
        /// <para>NOTE: If in Windows, cast thre return object to the System.Drawing.Image class.</para>
        /// </summary>
        public override object GetImage(string Key, CultureInfo Culture = null)
        {
            return GetObject(Key, Culture);
        }
    }
}
