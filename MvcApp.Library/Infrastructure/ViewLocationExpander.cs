namespace MvcApp.Library
{
    /// <summary>
    /// A location expander. It adds the views locations in the view search locations of the <see cref="RazorViewEngine"/>.
    /// <para>NOTE: Use the <c>static</c> <see cref="ViewLocationExpander.AddViewLocation(string)"/> method to add locations.</para>
    /// <para>NOTE: The order of view location matters.</para>
    /// <para>SEE: View Discavery at https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-9.0#view-discovery </para>
    /// </summary>
    public class ViewLocationExpander : IViewLocationExpander
    {
        const string SThemeKey = "__Theme__";
        static string fThemesFolder;

        static List<string> NonThemeableAreaList = new List<string>();
        static List<string> LocationList = new List<string>();

        // ● public 
        /// <summary>
        /// This is called by the <see cref="RazorViewEngine"/> when it looks to find a view (any kind of view, i.e. layout, normal view, or partial).
        /// <para>Information about the view, action and controller is passed in the <see cref="ViewLocationExpanderContext"/> parameter.</para>
        /// <para>This call gives a chance to search and locate the view and pass back the view location.</para>
        /// <para>NOTE: The order of view location matters.</para>
        /// </summary>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (LocationList.Count > 0)
                viewLocations = LocationList.Union(viewLocations);

            // 0 = view file name
            // 1 = controller name
            if (UseThemes && context.Values.TryGetValue(SThemeKey, out string Theme))
            {
                var Locations = new[] {
                        $"/{ThemesFolder}/{Theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/{ThemesFolder}/{Theme}/Views/Shared/{{0}}.cshtml",
                    };

                viewLocations = Locations.Union(viewLocations);
            }

            return viewLocations;
        }
        /// <summary>
        /// Invoked by a <see cref="RazorViewEngine"/> to determine the values that would be consumed by this instance
        /// of <see cref="IViewLocationExpander"/>. The calculated values are used to determine if the view location
        /// has changed since the last time it was located.
        /// <para>SEE: Remarks section at https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razor.iviewlocationexpander </para>
        /// </summary>
        /// <param name="context">The <see cref="ViewLocationExpanderContext"/> for the current view location
        /// expansion operation.</param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {

            if (!IsNonThemeableArea(context.AreaName)) // maybe there are some non-themeable areas
            {
                context.Values[SThemeKey] = Theme;
            }
        }

        // ● static  
        /// <summary>
        /// Adds a location to the internal list.
        /// <para>Example for normal views:
        /// <code>
        ///     // 0 = view file name
        ///     // 1 = controller name
        ///     
        ///     AddLocation("/Views/{1}/{0}.cshtml");   
        ///     AddLocation("/Views/Shared/{0}.cshtml");
        ///     AddLocation("/Views/Ajax/{0}.cshtml");
        /// </code>
        /// </para>
        /// <para>Example for themeable views:
        /// <code>
        ///     // 0 = view file name
        ///     // 1 = controller name
        /// 
        ///    AddLocation("/{ThemesFolder}/{Theme}/Views/{1}/{0}.cshtml");
        ///    AddLocation("/{ThemesFolder}/{Theme}/Views/Shared/{0}.cshtml");
        /// </code>
        /// </para>
        /// </summary>
        static public void AddViewLocation(string Location)
        {
            string S = LocationList.FirstOrDefault(s => string.Compare(s, Location, true) == 0);
            if (string.IsNullOrWhiteSpace(S))
                LocationList.Add(Location);
        }
        /// <summary>
        /// Adds the name of a non-themeable area to the internal list
        /// </summary>
        static public void AddNonThemeableArea(string AreaName)
        {
            if (!string.IsNullOrWhiteSpace(AreaName))
            {
                AreaName = AreaName.ToLowerInvariant();
                if (!NonThemeableAreaList.Contains(AreaName))
                    NonThemeableAreaList.Add(AreaName);
            }
        }
        /// <summary>
        /// Returns true if a specified area name refers to a non-themeable area
        /// </summary>
        static public bool IsNonThemeableArea(string AreaName)
        {
            if (!string.IsNullOrWhiteSpace(AreaName))
            {
                AreaName = AreaName.ToLowerInvariant();
                return NonThemeableAreaList.Contains(AreaName);
            }

            return false;
        }

        // ● properties  
        /// <summary>
        /// When true then themes are used
        /// </summary>
        static public bool UseThemes => !string.IsNullOrWhiteSpace(Theme);
        /// <summary>
        /// The theme currently in use. Defaults to null or empty string, meaning no themes at all.
        /// </summary>
        static public string Theme => Lib.Settings.Theme;
        /// <summary>
        /// The name of the Themes folder. Defaults to "Themes"
        /// </summary>
        static public string ThemesFolder
        {
            get { return !string.IsNullOrWhiteSpace(fThemesFolder) ? fThemesFolder : "Themes"; }
            set { fThemesFolder = value; }
        }
    }
}
