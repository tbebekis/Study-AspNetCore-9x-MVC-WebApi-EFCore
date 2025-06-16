namespace StudyLib
{
    /// <summary>
    /// Helper class for loading types and methods from assemblies
    /// </summary>
    static public class TypeFinder
    {

        /// <summary>
        /// Returns true if the specified Assembly is loadable and can be searched.
        /// </summary>
        static public bool CanSearchAssembly(Assembly Assembly)
        {
            if (Assembly.IsDynamic)
                return false;

            string Name = Assembly.FullName.ToUpperInvariant();

            return CanSearchAssembly(Name);
        }
        /// <summary>
        /// Returns true if the specified Assembly is loadable and can be searched.
        /// </summary>
        static public bool CanSearchAssembly(string AssemblyName)
        {
            if (AssemblyName.ContainsText("Microsoft")
                || AssemblyName.ContainsText("netstandard")
                || AssemblyName.ContainsText("Newtonsoft")
                )
                return false;

            foreach (string S in TypeFinderExcludedAssemblies)
            {
                if (AssemblyName.StartsWith(S, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns an array of the current domain assemblies. 
        /// <para>The result is sorted. Tripous assemblies come first of all.</para>
        /// <para>If SearchableOnly is true, then only those assemblies that pass the CanSearchAssembly() check are returned.</para>
        /// </summary>
        static public Assembly[] GetDomainAssemblies(bool SearchableOnly = true)
        {
            List<Assembly> List = new List<Assembly>();

            List.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            /* get searcable assemblies only? */
            if (SearchableOnly)
            {
                Assembly[] Items = List.ToArray();

                foreach (Assembly A in Items)
                {
                    if (!CanSearchAssembly(A))
                        List.Remove(A);
                }
            }

            return List.ToArray();
        }

        // ● class types derived from a base class 
        /// <summary>
        /// Returns a list of class types derived from BaseClass
        /// </summary>
        static public List<Type> FindDerivedClasses(Type BaseClass, List<Assembly> AssemblyList)
        {
            List<Type> Result = new List<Type>();
            foreach (Assembly Assembly in AssemblyList)
                FindDerivedClasses(BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a list of class types derived from BaseClass
        /// </summary>
        static public List<Type> FindDerivedClasses(Type BaseClass, Assembly Assembly)
        {
            List<Type> Result = new List<Type>();
            FindDerivedClasses(BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a list of class types derived from BaseClass
        /// </summary>
        static public void FindDerivedClasses(Type BaseClass, Assembly Assembly, List<Type> Result)
        {
            if (BaseClass.IsClass)
            {
                try
                {
                    Type[] Types = Assembly.GetTypesSafe();

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (T.IsClass && T.IsSubclassOf(BaseClass))
                                Result.Add(T);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }

        // ● class types implementing a certain interface type 
        /// <summary>
        /// Returns a list of class types implenting the InterfaceType interface
        /// </summary>
        static public List<Type> FindImplementorClasses(Type InterfaceType, List<Assembly> AssemblyList)
        {
            List<Type> Result = new List<Type>();
            foreach (Assembly Assembly in AssemblyList)
                FindImplementorClasses(InterfaceType, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a list of class types implenting the InterfaceType interface
        /// </summary>
        static public List<Type> FindImplementorClasses(Type InterfaceType, Assembly Assembly)
        {
            List<Type> Result = new List<Type>();
            FindImplementorClasses(InterfaceType, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a list of class types implenting the InterfaceType interface
        /// </summary>
        static public void FindImplementorClasses(Type InterfaceType, Assembly Assembly, List<Type> Result)
        {
            if (InterfaceType.IsInterface)
            {
                try
                {
                    Type[] Types = Assembly.GetTypesSafe();

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (T.IsClass && T.ImplementsInterface(InterfaceType))
                                Result.Add(T);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }


        // ● class types marked with a certain attribute
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type, of class types 
        /// marked with the AttributeType attribute.
        /// </summary>
        static public Dictionary<object, Type> FindClassesMarkedWith(Type AttributeType, List<Assembly> AssemblyList)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            foreach (Assembly Assembly in AssemblyList)
                FindClassesMarkedWith(AttributeType, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type, of class types 
        /// marked with the AttributeType attribute.
        /// </summary>
        static public Dictionary<object, Type> FindClassesMarkedWith(Type AttributeType, Assembly Assembly)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            FindClassesMarkedWith(AttributeType, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type, of class types 
        /// marked with the AttributeType attribute.
        /// </summary>
        static public void FindClassesMarkedWith(Type AttributeType, Assembly Assembly, Dictionary<object, Type> Result)
        {
            if (AttributeType.IsClass && AttributeType.InheritsFrom(typeof(Attribute)))
            {
                try
                {
                    Type[] Types = Assembly.GetTypesSafe();
                    Attribute Attr;

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (T.IsClass && T.IsDefined(AttributeType, false))
                            {
                                Attr = Attribute.GetCustomAttribute(T, AttributeType, false);
                                Result[Attr] = T;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }


        // ● static methods and/or constructors marked with a certain attribute
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a MethodBase 
        /// (actually a static method or a constructor), of MethodBase methods marked with the AttributeType attribute.
        /// </summary>
        static public Dictionary<object, MethodBase> FindMethodsMarkedWith(Type AttributeType, List<Assembly> AssemblyList, bool SelectStaticMethods = true, bool SelectConstructors = true)
        {
            Dictionary<object, MethodBase> Result = new Dictionary<object, MethodBase>();
            foreach (Assembly Assembly in AssemblyList)
                FindMethodsMarkedWith(AttributeType, Assembly, Result, SelectStaticMethods, SelectConstructors);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a MethodBase 
        /// (actually a static method or a constructor), of MethodBase methods marked with the AttributeType attribute.
        /// </summary>
        static public Dictionary<object, MethodBase> FindMethodsMarkedWith(Type AttributeType, Assembly Assembly, bool SelectStaticMethods = true, bool SelectConstructors = true)
        {
            Dictionary<object, MethodBase> Result = new Dictionary<object, MethodBase>();
            FindMethodsMarkedWith(AttributeType, Assembly, Result, SelectStaticMethods, SelectConstructors);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a MethodBase 
        /// (actually a static method or a constructor), of MethodBase methods marked with the AttributeType attribute.
        /// </summary>
        static public void FindMethodsMarkedWith(Type AttributeType, Assembly Assembly, Dictionary<object, MethodBase> Result, bool SelectStaticMethods = true, bool SelectConstructors = true)
        {
            if (AttributeType.IsClass && AttributeType.InheritsFrom(typeof(Attribute)))
            {
                if (!SelectStaticMethods && !SelectConstructors)
                    return;

                Attribute Attr;
                Action<MethodBase[]> FindMethods = delegate (MethodBase[] Methods)
                {
                    if ((Methods != null) && (Methods.Length > 0))
                    {
                        foreach (MethodBase Method in Methods)
                        {
                            try
                            {
                                Attr = Attribute.GetCustomAttribute(Method, AttributeType, false);
                                if (Attr != null)
                                {
                                    Result[Attr] = Method;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                };



                try
                {
                    Type[] Types = Assembly.GetTypesSafe();
                    MethodInfo[] Methods;
                    ConstructorInfo[] Constructors;

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (SelectStaticMethods)
                            {
                                Methods = T.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                                FindMethods(Methods);
                            }

                            if (SelectConstructors)
                            {
                                Constructors = T.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                FindMethods(Constructors);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }

        }


        // ● class types 1) marked with a certain attribute and 2) derived from a base class
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types marked with AttributeType and derived from BaseClass.
        /// <para>Searches all searchable domain assemblies.</para>
        /// </summary>
        static public Dictionary<object, Type> FindDerivedClasses(Type AttributeType, Type BaseClass)
        {
            List<Assembly> AssemblyList = new List<Assembly>(GetDomainAssemblies(true));
            return FindDerivedClasses(AttributeType, BaseClass, AssemblyList);
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types marked with AttributeType and derived from BaseClass.
        /// </summary>
        static public Dictionary<object, Type> FindDerivedClasses(Type AttributeType, Type BaseClass, List<Assembly> AssemblyList)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            foreach (Assembly Assembly in AssemblyList)
                FindDerivedClasses(AttributeType, BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types marked with AttributeType and derived from BaseClass.
        /// </summary>
        static public Dictionary<object, Type> FindDerivedClasses(Type AttributeType, Type BaseClass, Assembly Assembly)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            FindDerivedClasses(AttributeType, BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types marked with AttributeType and derived from BaseClass.
        /// </summary>
        static public void FindDerivedClasses(Type AttributeType, Type BaseClass, Assembly Assembly, Dictionary<object, Type> Result)
        {

            if (AttributeType.IsClass && AttributeType.InheritsFrom(typeof(Attribute)))
            {
                try
                {
                    Type[] Types = Assembly.GetTypesSafe();
                    Attribute Attr;

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (T.IsClass && T.IsDefined(AttributeType, false) && T.InheritsFrom(BaseClass))
                            {
                                Attr = Attribute.GetCustomAttribute(T, AttributeType, false);
                                Result[Attr] = T;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }

        }


        // ● class types 1) marked with a certain attribute, 2) implementing a certain interface type and 3) possibly derived from a base class
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types 1) marked with AttributeType 2) implementing a InterfaceType interface 
        /// and 3) if BaseClass is NOT null, derived from that BaseClass.
        /// </summary>
        static public Dictionary<object, Type> FindClasses(Type AttributeType, Type InterfaceType, Type BaseClass, List<Assembly> AssemblyList)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            foreach (Assembly Assembly in AssemblyList)
                FindClasses(AttributeType, InterfaceType, BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types 1) marked with AttributeType 2) implementing a InterfaceType interface 
        /// and 3) if BaseClass is NOT null, derived from that BaseClass.
        /// </summary>
        static public Dictionary<object, Type> FindClasses(Type AttributeType, Type InterfaceType, Type BaseClass, Assembly Assembly)
        {
            Dictionary<object, Type> Result = new Dictionary<object, Type>();
            FindClasses(AttributeType, InterfaceType, BaseClass, Assembly, Result);
            return Result;
        }
        /// <summary>
        /// Returns a dictionary where Key is an Attribute instance and Value a class type,
        /// of class types 1) marked with AttributeType 2) implementing a InterfaceType interface 
        /// and 3) if BaseClass is NOT null, derived from that BaseClass.
        /// </summary>
        static public void FindClasses(Type AttributeType, Type InterfaceType, Type BaseClass, Assembly Assembly, Dictionary<object, Type> Result)
        {
            if (AttributeType.IsClass && AttributeType.InheritsFrom(typeof(Attribute)) && InterfaceType.IsInterface)
            {
                try
                {
                    Type[] Types = Assembly.GetTypesSafe();
                    Attribute Attr;

                    foreach (Type T in Types)
                    {
                        try
                        {
                            if (T.IsClass
                                && T.IsDefined(AttributeType, false)
                                && T.ImplementsInterface(InterfaceType)
                                && ((BaseClass == null) || T.InheritsFrom(BaseClass)))
                            {
                                Attr = Attribute.GetCustomAttribute(T, AttributeType, false);
                                Result[Attr] = T;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }


        }

        /// <summary>
        /// Contains the starting part of assembly names.
        /// <para>Assemblies found in this list, are excluded by the registration searching.</para>
        /// <para>Defaults to System, Microsoft, mscorlib and vshost. </para>
        /// </summary>
        static public List<string> TypeFinderExcludedAssemblies { get; private set; } = new List<string>(new string[] { "System", "Microsoft", "mscorlib", "vshost" });

    }
}
