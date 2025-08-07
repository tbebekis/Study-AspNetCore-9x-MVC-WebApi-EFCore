namespace CommonLib.Logging
{
    /// <summary>
    /// The information of a log entry.
    /// <para>The logger creates an instance of this class when its Log() method is called, 
    /// fills the properties and then passes the info to the provider calling WriteLog(). </para>
    /// </summary>
    public class LogEntry
    {
        static Dictionary<string, int> fLineLengths = new Dictionary<string, int>();
        static string fLineCaptions;

        string fAsLine;
        string fAsList;
        string fAsJson;
        JsonObject fJsonObject;

        string Pad(string Text, int MaxLength)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return "".PadRight(MaxLength);

            if (Text.Length > MaxLength)
                return Text.Substring(0, MaxLength);

            return Text.PadRight(MaxLength);
        }
        List<string> GetStatePropertiesList()
        {
            List<string> List = new();

            if (StateProperties != null && StateProperties.Count > 0)
            {
                foreach (var Pair in StateProperties)
                    List.Add($"{Pair.Key} = {Pair.Value.ToString()}");
            }

            return List;
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public LogEntry()
        {
            TimeStampUtc = DateTime.UtcNow;
            UserName = Environment.UserName;
 
            Id = Guid.NewGuid().ToString("D").ToUpper(); // GUID string without brackets
        }
        /// <summary>
        /// Static Constructor
        /// </summary>
        static LogEntry()
        {
            // prepare the lengs table
            fLineLengths.Add("Id", 40);
            fLineLengths.Add("TimeStamp", 24);
            fLineLengths.Add("Host", 24);
            fLineLengths.Add("User", 24);
            fLineLengths.Add("Level", 12);
            fLineLengths.Add("EventId", 14);
            fLineLengths.Add("Category", 92);
            fLineLengths.Add("Scope", 64);
        }

        // ● public 
        /// <summary>
        /// Returns the last scope. The last scope is the current one.
        /// </summary>
        public string GetScopeText()
        {
            string Result = "";
            if (Scopes != null && Scopes.Count > 0)
            {
                LogScopeInfo SI = Scopes.Last();
                if (!string.IsNullOrWhiteSpace(SI.Text))
                    Result = SI.Text;
            }

            return Result;
        }
        /// <summary>
        /// Returns a string representation of the <see cref="StateProperties"/> property of this entry.
        /// </summary>
        public string GetPropertiesAsSingleLine()
        {
            string Result = string.Empty;
            List<string> List = GetStatePropertiesList();
            if (List != null && List.Count > 0) 
                Result = string.Join(", ", List);
            return Result;
        }
        /// <summary>
        /// Returns a string representation of the <see cref="StateProperties"/> property of this entry.
        /// </summary>
        public string GetPropertiesAsTextList()
        {
            string Result = string.Empty;
            List<string> List = GetStatePropertiesList();
            if (List != null && List.Count > 0)
                Result = string.Join(Environment.NewLine, List);
            return Result;
        }

        // ● properties 
        /// <summary>
        /// The entry Id. A GUID string without brackets.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// User name of the person who is currently logged on to operating system.
        /// </summary>
        public string UserName { get; }
        /// <summary>
        /// Host name of local computer
        /// </summary>
        public string HostName => StaticHostName;
        /// <summary>
        /// Date and time, in UTC, of the creation time of this instance
        /// </summary>
        public DateTime TimeStampUtc { get; }
        /// <summary>
        /// The timestamp as string.
        /// </summary>
        public string TimeStampText => TimeStampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff");
        /// <summary>
        /// The source that created this log entry.
        /// <para>Asp.Net Core calls it <c>Category</c>.</para>
        /// <para>The source is usually the fully qualified class name of a class using a logger, e.g. MyNamespace.MyClass </para>
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The log level of this information.
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// The message of this information
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The exception this information represents, if any, else null.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// The exception data, if this is a log regarding an exception
        /// </summary>
        public string ExceptionData => Exception == null ? string.Empty : Exception.GetFullText();
        /// <summary>
        /// The EventId of this information. 
        /// <para>An EventId with Id set to zero, usually means no EventId.</para>
        /// </summary>
        public EventId EventId { get; set; }
        /// <summary>
        /// The state object. Contains information regarding the text message.
        /// <para>It looks like its type is always Microsoft.Extensions.Logging.Internal.FormattedLogValues </para>
        /// </summary>
        public object State { get; set; }
        /// <summary>
        /// Used when State is just a string type. So far null.
        /// </summary>
        public string StateText { get; set; }
        /// <summary>
        /// A dictionary with State properties.
        /// <para>When the log message is a message template with format values, e.g. <code>Logger.LogInformation("Customer {CustomerId} order {OrderId} is completed", CustomerId, OrderId)</code>  </para>
        /// this dictionary contains entries gathered from the message in order to ease any Structured Logging providers.
        /// </summary>
        public Dictionary<string, object> StateProperties { get; set; }
        /// <summary>
        /// The scopes currently in use, if any. The last scope is the current one.
        /// </summary>
        public List<LogScopeInfo> Scopes { get; set; }

        /// <summary>
        /// Returns the host name of the local computer
        /// </summary>
        static public readonly string StaticHostName = System.Net.Dns.GetHostName();
        /// <summary>
        /// Returns a string with the captions of the log information, property formatted, i.e. right padded with spaces.
        /// </summary>
        static public string LineCaptions
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fLineCaptions))
                {
                    StringBuilder SB = new StringBuilder();

                    SB.Append("Id".PadRight(fLineLengths["Id"]));
                    SB.Append("TimeStamp UTC".PadRight(fLineLengths["TimeStamp"]));
                    SB.Append("Host".PadRight(fLineLengths["Host"]));
                    SB.Append("User".PadRight(fLineLengths["User"]));
                    SB.Append("Level".PadRight(fLineLengths["Level"]));
                    SB.Append("EventId".PadRight(fLineLengths["EventId"]));
                    SB.Append("Category".PadRight(fLineLengths["Category"]));
                    SB.Append("Scope".PadRight(fLineLengths["Scope"]));
                    SB.Append("Text");
                    SB.AppendLine();

                    fLineCaptions = SB.ToString();
                }

                return fLineCaptions;

            }
        }

        /// <summary>
        /// Returns a string representation of this entry.
        /// </summary>
        public string AsLine
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fAsLine))
                {
                    string RemoveLineEndings(string S)
                    {
                        return S.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
                    }

                    StringBuilder SB = new StringBuilder();

                    SB.Append(Pad(this.Id,                    fLineLengths["Id"]));
                    SB.Append(Pad(this.TimeStampText,         fLineLengths["TimeStamp"]));
                    SB.Append(Pad(this.HostName,              fLineLengths["Host"]));
                    SB.Append(Pad(this.UserName,              fLineLengths["User"]));
                    SB.Append(Pad(this.Level.ToString(),      fLineLengths["Level"]));
                    SB.Append(Pad(this.EventId.Id.ToString(), fLineLengths["EventId"]));
                    SB.Append(Pad(this.Source,              fLineLengths["Category"]));
                    SB.Append(Pad(this.GetScopeText(),        fLineLengths["Scope"])); 

                    if (!string.IsNullOrWhiteSpace(this.Text))
                        SB.Append(RemoveLineEndings(this.Text));

                    if (!string.IsNullOrWhiteSpace(this.ExceptionData))
                        SB.Append(RemoveLineEndings(this.ExceptionData));

                    string LineText = SB.ToString();

                    if ((this.StateProperties != null) && (this.StateProperties.Count > 0))
                    {
                        string PropertyText = this.GetPropertiesAsSingleLine();
                        if (!string.IsNullOrWhiteSpace(PropertyText))
                        {
                            LineText += " - Properties: ";
                            LineText += PropertyText;
                        }
                    }

                    fAsLine = LineText;
                }

                return fAsLine;
            }
        }
        /// <summary>
        /// Returns a string representation of this entry.
        /// </summary>
        public string AsList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fAsList))
                {

                    void AddLine(StringBuilder SB, string Name, string Value)
                    {
                        if (!string.IsNullOrWhiteSpace(Value))
                        {
                            Value = Name.PadRight(12) + ": " + Value;
                            SB.Append(Value);
                        }
                    }

                    StringBuilder SB = new StringBuilder();
                    AddLine(SB, "Id", this.Id);
                    AddLine(SB, "TimeStamp", this.TimeStampText);
                    AddLine(SB, "Level", this.Level.ToString());
                    AddLine(SB, "Category", this.Source);
                    AddLine(SB, "Scope", this.GetScopeText());
                    AddLine(SB, "EventId", this.EventId.Id.ToString());
                    AddLine(SB, "Host", this.HostName);
                    AddLine(SB, "User", this.UserName);
                    AddLine(SB, "Text", this.Text);

                    if (!string.IsNullOrWhiteSpace(ExceptionData))
                        AddLine(SB, "Stack", Environment.NewLine + ExceptionData);

                    if ((this.StateProperties != null) && (this.StateProperties.Count > 0))
                    {
                        AddLine(SB, "Properties", " ");
                        SB.AppendLine(this.GetPropertiesAsTextList());
                    }

                    fAsList = SB.ToString();
                }

                return fAsList;
            }


        }
        /// <summary>
        /// Returns a string representation of this entry.
        /// </summary>
        public string AsJson
        {
            get
            {
                if (fAsJson == null)
                {
                    fAsJson = AsJsonObject.ToJsonString();
                }

                return fAsJson;
            }
        }
        /// <summary>
        /// Returns a <see cref="JsonObject"/> representation of this entry.
        /// </summary>
        public JsonObject AsJsonObject
        {
            get
            {
                if (fJsonObject == null)
                {
                    fJsonObject = new();

                    fJsonObject.Add("Id", JsonValue.Create(this.Id));
                    fJsonObject.Add("TimeStamp", JsonValue.Create(this.TimeStampText));
                    fJsonObject.Add("Level", JsonValue.Create(this.Level.ToString()));
                    fJsonObject.Add("Category", JsonValue.Create(this.Source));
                    fJsonObject.Add("Scope", JsonValue.Create(this.GetScopeText()));
                    fJsonObject.Add("EventId", JsonValue.Create(this.EventId.Id.ToString()));
                    fJsonObject.Add("Host", JsonValue.Create(this.HostName));
                    fJsonObject.Add("User", JsonValue.Create(this.UserName));
                    fJsonObject.Add("Text", JsonValue.Create(this.Text));
                    fJsonObject.Add("Stack", JsonValue.Create(this.ExceptionData));
                    fJsonObject.Add("Properties", JsonValue.Create(this.GetPropertiesAsTextList()));
                }

                return fJsonObject;
            }
        }

    }
}
