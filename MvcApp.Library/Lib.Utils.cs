using Microsoft.AspNetCore.Routing;

namespace MvcApp.Library
{
    static public partial class Lib
    {

        // ● strings 
        /// <summary>
        /// Case insensitive string equality.
        /// </summary>
        public static bool IsSameText(string A, string B)
        {
            return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        // ● files and folders
        /// <summary>
        /// Returns true if a specified path is a directory
        /// </summary>
        static public bool IsDirectory(string FolderPath)
        {
            return Directory.Exists(FolderPath);
        }
        /// <summary>
        /// Deletes a folder
        /// <para>Waits until the directory is actually deleted and not just marked as deleted</para>
        /// </summary>
        static public void DeleteFolder(string FolderPath)
        {
            Directory.Delete(FolderPath, true);
            int MaxIterations = 10;
            int Counter = 0;

            // wait until the directory is actually deleted
            // and not just marked as deleted
            while (Directory.Exists(FolderPath))
            {
                Counter += 1;

                if (Counter > MaxIterations)
                    return;

                Thread.Sleep(250);
            }
        }

        // ● to/from Base64  
        /// <summary>
        /// Encodes Value into a Base64 string using the specified Enc.
        /// If End is null, the Encoding.Unicode is used.
        /// </summary>
        static public string StringToBase64(string Value, Encoding Enc)
        {
            if (Enc == null)
                Enc = Encoding.Unicode;

            byte[] Data = Enc.GetBytes(Value);
            return Convert.ToBase64String(Data);
        }
        /// <summary>
        /// Decodes the Base64 string Value into a string using the specified Enc.
        /// If End is null, the Encoding.Unicode is used.
        /// </summary>
        static public string Base64ToString(string Value, Encoding Enc)
        {
            if (Enc == null)
                Enc = Encoding.Unicode;

            byte[] Data = Convert.FromBase64String(Value);
            return Enc.GetString(Data);
        }

        // ● miscs 
        /// <summary>
        /// Reads and returns an HTTP header from <see cref="HttpRequest.Headers"/>
        /// </summary>
        static public string GetHttpHeader(this HttpRequest Request, string Key)
        {
            Key = Key.ToLowerInvariant();
            return Request == null ? string.Empty : Request.Headers.FirstOrDefault(x => x.Key.ToLowerInvariant() == Key).Value.FirstOrDefault();
        }

        /// <summary>
        /// Creates and returns a new Guid.
        /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
        /// </summary>
        static public string GenId(bool UseBrackets)
        {
            string format = UseBrackets ? "B" : "D";
            return Guid.NewGuid().ToString(format).ToUpper();
        }
        /// <summary>
        /// Creates and returns a new Guid WITHOUT surrounding brackets, i.e. {}
        /// </summary>
        static public string GenId()
        {
            return GenId(false);
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        static public void Throw(string ErrorMessage)
        {
            throw new Exception(ErrorMessage);
        }

        /// <summary>
        /// Returns a localized string based on a specified resource key, e.g. Customer, and the culture code of the current request, e.g. el-GR
        /// </summary>
        static public string Localize(string Key, string Default = "")
        {
            if (string.IsNullOrWhiteSpace(Default))
                Default = Key;

            return Res.GetString(Key, Default, Lib.Culture);
        }

        /// <summary>
        /// Returns a list with the end points defined in this application
        /// </summary>
        static public List<string> GetEndPointList()
        {
            EndpointDataSource endpointDataSource = Lib.GetService<EndpointDataSource>();

            List<string> EndPointList = new();

            RouteEndpoint REP;
            string DisplayName;
            string Pattern;
            string S;

            foreach (var EP in endpointDataSource.Endpoints)
            {
                REP = EP as RouteEndpoint;
                if (REP != null)
                {
                    DisplayName = !string.IsNullOrWhiteSpace(REP.DisplayName) ? REP.DisplayName : "no name";
                    Pattern = !string.IsNullOrWhiteSpace(REP.RoutePattern.RawText) ? REP.RoutePattern.RawText : "no pattern";
                    S = $"DisplayName = {DisplayName}, Pattern = {Pattern}";
                    EndPointList.Add(S);

                    if (Pattern == "product/paging")
                    {
                        //var xxx = 123;
                        foreach (var Metadata in REP.Metadata)
                        {
                            EndPointList.Add(Metadata.GetType().FullName);
                        }
                    }
                }
            }

            return EndPointList;
        }

        // ● properties
        static public bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.WinCE;
        static public bool IsLinux => Environment.OSVersion.Platform == PlatformID.Unix;

    }
}
