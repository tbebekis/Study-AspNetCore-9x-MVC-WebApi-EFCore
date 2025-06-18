namespace CommonLib
{

    /// <summary>
    /// Provides access to the resources of any <see cref="IResourceProvider"/> registered by calling the Add method.
    /// </summary>
    static public class Res
    {
        /// <summary>
        /// Field
        /// </summary>
        static List<IResourceProvider> ResourceProviders = new List<IResourceProvider>();


        // ● construction
        /// <summary>
        /// Static constructor.
        /// </summary>
        static Res()
        {
        }

        // ● public 
        /// <summary>
        /// Adds Provider to the internal resource providers list.
        /// </summary>
        static public void Add(IResourceProvider Provider)
        {
            if (Provider != null && !ResourceProviders.Contains(Provider) && !string.IsNullOrWhiteSpace(Provider.Name))
            {
                IResourceProvider Item = ResourceProviders.FirstOrDefault(x => string.Compare(x.Name, Provider.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (Item == null)
                    ResourceProviders.Insert(0, Provider);
            }
        }
        /// <summary>
        /// Creaetes a resource provider based on the specified arguments and
        /// adds that provider to the internal provider list.
        /// </summary>
        static public void Add(ResourceManager Manager, string Name)
        {
            if ((Manager != null) && !string.IsNullOrWhiteSpace(Name))
            {
                Add(new ResourceProviderWithResourceManager(Manager, Name));
            }
        }

        // ● strings 
        /// <summary>
        /// Returns a resource string for the Key, if any, else Default.
        /// </summary>
        static public string GetString(string Key, string Default, CultureInfo Culture = null)
        {
            string Result;

            foreach (IResourceProvider RP in ResourceProviders)
            {
                Result = RP.GetString(Key, Culture);
                if (!string.IsNullOrWhiteSpace(Result))
                    return Result;
            }

            return Default;
        }
        /// <summary>
        /// Returns a resource string for the Key, if any, else Default.
        /// </summary>
        static public string GetString(string Key, CultureInfo Culture = null)
        {
            return GetString(Key, Key, Culture);
        }
        
        /// <summary>
        /// Returns a resource string for the Key, if any, else Default.
        /// <para>L stands for Localize.</para>
        /// </summary>
        static public string L(string Key, string Default, CultureInfo Culture = null)
        {
            return GetString(Key, Default, Culture);
        }
        /// <summary>
        /// Returns a resource string for the Key, if any, else returns the Key.
        /// <para>L stands for Localize.</para>
        /// </summary>
        static public string L(string Key, CultureInfo Culture = null)
        {
            return GetString(Key, Key, Culture);
        }

        // ● non-strings 
        /// <summary>
        /// Returns a resource object for the Key, if any, else null.
        /// </summary>
        static public object GetObject(string Key, CultureInfo Culture = null)
        {
            object Result;
            foreach (IResourceProvider RP in ResourceProviders)
            {
                Result = RP.GetObject(Key, Culture);
                if (Result != null)
                    return Result;
            }

            return null;
        }
        /// <summary>
        /// Returns a binary resource for the Key, if any, else null.
        /// </summary>
        static public byte[] GetBinary(string Key, CultureInfo Culture = null)
        {
            byte[] Data;
            foreach (IResourceProvider RP in ResourceProviders)
            {
                Data = RP.GetBinary(Key, Culture);
                if (Data != null)
                    return Data;
            }

            return null;
        }
        /// <summary>
        /// Returns a resource Image for the Key, if any, else null.
        /// <para>NOTE: If in Windows, cast thre return object to the System.Drawing.Image class.</para>
        /// </summary>
        static public object GetImage(string Key, CultureInfo Culture = null)
        {
            object Data;
            foreach (IResourceProvider RP in ResourceProviders)
            {
                Data = RP.GetImage(Key, Culture);
                if (Data != null)
                    return Data;
            }

            return null;
        }

        /// <summary>
        /// Converts an xml file created using DataTable.WriteXml() and saved as a resource under Key, 
        /// back to a DataTable again.
        /// </summary>
        static public DataTable GetDataTable(string Key, CultureInfo Culture = null)
        {
            DataTable Table = new DataTable("");

            string XmlText = GetObject(Key, Culture) as string;

            if (!string.IsNullOrWhiteSpace(XmlText))
            {
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(XmlText);

                using (MemoryStream MS = new MemoryStream())
                {
                    Doc.Save(MS);
                    MS.Position = 0;

                    Table.ReadXml(MS);
                    Table.AcceptChanges();
                }
            }

            return Table;

        }

        // ● miscs 
        /// <summary>
        /// Saves data designated by Key to FileName
        /// </summary>
        static public bool SaveToFile(string Key, string FileName)
        {

            if (!File.Exists(FileName))
            {
                object ResData = GetObject(Key);
                if (ResData != null)
                {
                    if (ResData.GetType() == typeof(System.String))
                    {
                        using (StreamWriter SW = new StreamWriter(FileName))
                        {
                            SW.Write(ResData);
                        }
                    }
                    else if (ResData.GetType() == typeof(byte[]))
                    {
                        byte[] Bytes = (byte[])ResData;

                        using (FileStream stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            stream.Write(Bytes, 0, Bytes.Length);
                        }
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (!File.Exists(FileName))
                            Thread.Sleep(500);
                        else
                            break;
                    }

                }


            }

            return File.Exists(FileName);

        }

 
    }
}
