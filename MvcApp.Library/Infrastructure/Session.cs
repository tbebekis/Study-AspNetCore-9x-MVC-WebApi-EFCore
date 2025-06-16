namespace MvcApp.Library 
{
    /// <summary>
    /// Provides access to session variables (entries)
    /// </summary>
    static public class Session
    {
        static IHttpContextAccessor HttpContextAccessor;
        /// <summary>
        /// Returns the current <see cref="HttpContext"/>
        /// </summary>
        static public HttpContext GetHttpContext() => HttpContextAccessor.HttpContext;

        // ● private
        /// <summary>
        /// Returns a value stored in session, found under a specified key or a default value if not found.
        /// </summary>
        static T Get<T>(this ISession Instance, string Key)
        {
            Key = Key.ToLowerInvariant();
            string JsonText = Instance.GetString(Key);
            if (JsonText == null)
                return default(T);

            return Json.Deserialize<T>(JsonText);
        }
        /// <summary>
        /// Returns a value stored in session, found under a specified key or a default value if not found.
        /// </summary>
        static T Get<T>(this ISession Instance, string Key, T Default)
        {
            Key = Key.ToLowerInvariant();
            string JsonText = Instance.GetString(Key);
            if (JsonText == null)
                return Default;

            return Json.Deserialize<T>(JsonText);
        }
        /// <summary>
        /// Stores a value in session under a specified key.
        /// </summary>
        static void Set<T>(this ISession Instance, string Key, T Value)
        {
            Key = Key.ToLowerInvariant();
            string JsonText = Json.Serialize(Value);
            Instance.SetString(Key, JsonText);
        }

        // ● public
        /// <summary>
        /// Initializes this class
        /// </summary>
        static public void Initialize(IHttpContextAccessor HttpContextAccessor)
        {
            Session.HttpContextAccessor = HttpContextAccessor;
        }


        /// <summary>
        /// Returns the session object
        /// </summary>
        static public ISession GetSession()
        {
            return GetHttpContext().Session;
        }

        /// <summary>
        /// Returns a value stored in session, found under a specified key or a default value if not found.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public T Get<T>(string Key)
        {
            ISession Session = GetSession();
            return Session != null ? Session.Get<T>(Key) : default(T);
        }
        /// <summary>
        /// Returns a value stored in session, found under a specified key or a default value if not found.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public T Get<T>(string Key, T Default)
        {
            ISession Session = GetSession();
            return Session != null ? Session.Get<T>(Key, Default) : default(T);
        }
        /// <summary>
        /// Stores a value in session under a specified key.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// <para>WARNING: Whenever an object is added by calling Set(), the object is serialized.
        /// So adding the object first and then altering the object will NOT work.
        /// The object should be added after any alteration to it is done.</para>
        /// </summary>
        static public void Set<T>(string Key, T Value)
        {
            ISession Session = GetSession();
            if (Session != null)
                Session.Set(Key, Value);
        }

        /// <summary>
        /// Removes and returns a value found under a specified key, if any, else returns the default value of the specified type argument.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public T Pop<T>(string Key)
        {
            T Result = Get<T>(Key);
            Remove(Key);
            return Result;
        }

        /// <summary>
        /// Returns a string stored in session, found under a specified key or null if not found.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public string GetString(string Key)
        {
            return Get<string>(Key, null);
        }
        /// <summary>
        /// Stores a string value in session under a specified key.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public void SetString(string Key, string Value)
        {
            Set(Key, Value);
        }

        /// <summary>
        /// Clears all session variables
        /// </summary>
        static public void Clear()
        {
            ISession Session = GetSession();
            if (Session != null)
                Session.Clear();
        }
        /// <summary>
        /// Removes a session variable under a specified key.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public void Remove(string Key)
        {
            ISession Session = GetSession();
            if (Session != null)
            {
                Key = Key.ToLowerInvariant();
                Session.Remove(Key);
            }
        }
        /// <summary>
        /// Returns true if a variable exists in session under a specified key.
        /// <para>NOTE: Key is NOT case sensitive.</para>
        /// </summary>
        static public bool ContainsKey(string Key)
        {
            ISession Session = GetSession();
            if (Session != null)
            {
                Key = Key.ToLowerInvariant();
                byte[] Buffer = null;
                return Session.TryGetValue(Key, out Buffer);
            }

            return false;
        }

        // ● error list and success list 
        /// <summary>
        /// Returns a <see cref="List{T}"/>    found under a specified key in session variables.
        /// </summary> 
        static List<string> GetSessionStringList(string Key)
        {
            List<string> List = null;
            if (Session.ContainsKey(Key))
                List = Session.Get<List<string>>(Key);

            if (List == null)
                List = new List<string>();

            return List;
        }
        /// <summary>
        /// Adds a message to SuccessList
        /// <para>NOTE: SuccessList and ErrorList messages are displayed to the user until lists are Pop()-ed.</para>
        /// </summary>
        static public void AddToSuccessList(string Message)
        {
            List<string> List = GetSessionStringList("SuccessList");
            List.Add(Message);
            Session.Set<List<string>>("SuccessList", List);
        }
        /// <summary>
        /// Adds a message to ErrorList
        /// <para>NOTE: SuccessList and ErrorList messages are displayed to the user until lists are Pop()-ed.</para>
        /// </summary>
        static public void AddToErrorList(string Message)
        {
            List<string> List = GetSessionStringList("ErrorList");
            List.Add(Message);
            Session.Set<List<string>>("ErrorList", List);
        }
        /// <summary>
        /// Adds a list of messages to ErrorList
        /// <para>NOTE: SuccessList and ErrorList messages are displayed to the user until lists are Pop()-ed.</para>
        /// </summary>
        static public void AddToErrorList(List<string> MessageList)
        {
            List<string> List = GetSessionStringList("ErrorList");
            List.AddRange(MessageList.ToArray());
            Session.Set<List<string>>("ErrorList", List);
        }

        /// <summary>
        /// Removes and returns the SuccessList.
        /// <para>NOTE: SuccessList and ErrorList messages are displayed to the user until lists are Pop()-ed.</para>
        /// </summary>
        static public List<string> PopSuccessList()
        {
            return Session.Pop<List<string>>("SuccessList");
        }
        /// <summary>
        /// Removes and returns the ErrorList.
        /// <para>NOTE: SuccessList and ErrorList messages are displayed to the user until lists are Pop()-ed.</para>
        /// </summary>
        static public List<string> PopErrorList()
        {
            return Session.Pop<List<string>>("ErrorList");
        }

        // ● properties
        /// <summary>
        /// Provides acces to request variables.
        /// <para>This dictionary is used to store data while processing a single request. The dictionary's contents are discarded after a request is processed.</para>
        /// </summary>
        static public IDictionary<object, object> Request
        {
            get
            {
                HttpContext HttpContext = GetHttpContext();
                return HttpContext != null ? HttpContext.Items : new Dictionary<object, object>();
            }
        }


    }
}
