namespace MvcApp.Library
{
    public class DataContext : DbContext
    {
        public const string SMemoryDatabase = "MemoryDatabase";

        // ● overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Product).Assembly);

            if (!UseInMemoryDatabase)
            {
                DemoData.AddData(modelBuilder);
            }

            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (UseInMemoryDatabase)
            {
                optionsBuilder.UseInMemoryDatabase(DataContext.SMemoryDatabase);
            }
            else
            {
                string DatabasePath = Path.Combine(System.AppContext.BaseDirectory, "Sqlite.db3");
                optionsBuilder.UseSqlite($"Data Source={DatabasePath}", SqliteOptionsBuilder => { });
            }

            
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public DataContext()
            : this(new DbContextOptions<DataContext>())
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// If the DbContext subtype is itself intended to be inherited from, then it should expose a protected constructor taking a non-generic DbContextOptions
        /// SEE: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontextoptions-versus-dbcontextoptionstcontext
        /// </summary>
        protected DataContext(DbContextOptions contextOptions)
        : base(contextOptions)
        {
        }

        // ● public
        static public void EnsureDatabase()
        {
            using (var context = new DataContext())
            {
                context.Database.EnsureCreated();
            }
        }
        static public bool UseInMemoryDatabase { get; set; }
    }
}
