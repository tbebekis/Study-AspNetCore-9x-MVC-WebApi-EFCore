namespace MvcApp.Library
{
    public class VisitorService : AppDataService<Visitor>
    {
        public VisitorService()
        {
        }

        /// <summary>
        /// Returns the visitor based on a specified Id, if any, else null.
        /// </summary>
        public Visitor GetVisitor(string Code)
        {
            Visitor Result = null;
            try
            {
                using (var DataContext = GetDataContext())
                {
                    DbSet<Visitor> DbSet = DataContext.Set<Visitor>();
                    Result = DbSet.AsNoTracking().FirstOrDefault(x => x.Id == Code);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return Result;
        }
        /// <summary>
        /// Inserts or updates a visitor and returns the Visitor
        /// </summary>
        public Visitor SaveVisitor(Visitor Visitor)
        {
            Visitor Result = null;
            try
            {
 
                using (var DataContext = GetDataContext())
                {
                    Visitor.IpAddress = WebLib.GetClientIpAddress();

                    DbSet<Visitor> DbSet = DataContext.Set<Visitor>();
                    if (Visitor.IsNew())
                    {
                        Visitor.SetId();
                        DataContext.Add(Visitor);
                    }
                    else
                    {
                        DataContext.Update(Visitor);
                    }

                    DataContext.SaveChanges();

                    Result = DbSet.AsNoTracking().FirstOrDefault(x => x.Id == Visitor.Id);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return Result; 
        }

        /// <summary>
        /// Returns the visitor based on a specified User Id, if any, else null.
        /// <para>When a visitor is registered is associated to a user.</para>
        /// </summary>
        public Visitor GetVisitorByAppUser(string UserId)
        {
            Visitor Result = null;
            try
            {
                using (var DataContext = GetDataContext())
                {
                    DbSet<Visitor> DbSet = DataContext.Set<Visitor>();
                    Result = DbSet.AsNoTracking().FirstOrDefault(x => x.UserId == UserId);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return Result;
        }

        /// <summary>
        /// Creates and returns a new Visitor.
        /// <para>The new Visitor is saved in the database.</para>
        /// </summary>
        public Visitor CreateNewVisitor()
        {
            Visitor Visitor = new Visitor();
            Visitor Result = SaveVisitor(Visitor);
            return Result;
        }
    }
}

 