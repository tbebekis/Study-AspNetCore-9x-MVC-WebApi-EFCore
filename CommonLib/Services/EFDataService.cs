namespace CommonLib
{

    /// <summary>
    /// A generic data service for entities derived from <see cref="BaseEntity"/>
    /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
    /// <para>NOTE: This service should be used with <strong>Disconnected Entities</strong>, SEE: https://learn.microsoft.com/en-us/ef/core/saving/disconnected-entities </para>
    /// </summary>
    public class EFDataService<T> where T : BaseEntity
    {
        //Type fDataContextType;
        string fTableName;
        Func<DbContext> GetDataContextFunc;

        protected virtual void CheckCRUDMode(CRUDMode Mode)
        {
            bool IsSet = (AllowedCRUDModes & Mode) == Mode;
            if (!IsSet)
                throw new ApplicationException($"CRUD mode not supported: {Mode}");
        }

        // ● (overridables) get entity lists    
        protected virtual async Task<List<T>> GetListAsync(
            DbContext DataContext,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetList(null, OrderByFunc, IncludeFuncs);
        }
        protected virtual async Task<List<T>> GetListWithFilterAsync(
            DbContext DataContext,
            Expression<Func<T, bool>> FilterFunc,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetListWithFilter(FilterFunc, null, OrderByFunc, IncludeFuncs);
        }
        protected virtual async Task<List<T>> GetListWithSqlFilterAsync(
            DbContext DataContext,
            string SqlWhereText,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetListWithSqlFilter(SqlWhereText, null, OrderByFunc, IncludeFuncs);
        }

        // ● (overridables) get entity lists with pagination
        protected virtual async Task<List<T>> GetListPagedAsync(
            DbContext DataContext,
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetList(Paging, OrderByFunc, IncludeFuncs);
        }
        protected virtual async Task<List<T>> GetListWithFilterPagedAsync(
            DbContext DataContext,
            IPaging Paging,
            Expression<Func<T, bool>> FilterFunc,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetListWithFilter(FilterFunc, Paging, OrderByFunc,  IncludeFuncs);
        }
        protected virtual async Task<List<T>> GetListWithSqlFilterPagedAsync(
            DbContext DataContext,
            IPaging Paging,
            string SqlWhereText,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            return await DataContext.GetListWithSqlFilter(SqlWhereText, Paging, OrderByFunc, IncludeFuncs);
        }

        // ● get single entity
        /// <summary>
        /// Returns an entity by its Id primary key.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<T> GetByIdAsync(DbContext DataContext, string Id)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<T> GetByProcAsync(DbContext DataContext, Func<T, bool> Proc)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            //return await DbSet.AsNoTracking().Where(x => Proc(x)).FirstOrDefaultAsync();    //return DbSet.Where(Proc).FirstOrDefault();
            List<T> List = await DbSet.AsNoTracking().ToListAsync();
            return List.FirstOrDefault(Proc);
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// <para>NOTE: if the specified entity comes without an Id, a new one Id is assigned to it.</para>
        /// </summary>
        protected virtual T Insert(DbContext DataContext, T Entity)
        {
            if (string.IsNullOrWhiteSpace(Entity.Id))
                Entity.SetId();

            EntityEntry<T> Entry = DataContext.Add(Entity);
            return Entry.Entity;
        }
        /// <summary>
        /// Updates an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual T Update(DbContext DataContext, T Entity)
        {
            EntityEntry<T> Entry = DataContext.Update(Entity);
            return Entry.Entity;
        }
        /// <summary>
        /// Deletes an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual T Delete(DbContext DataContext, T Entity)
        {
            EntityEntry<T> Entry = DataContext.Remove(Entity);
            return Entry.Entity;
        }


        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public EFDataService()
        {
            Type ClassType = typeof(T);
            if (ClassType.IsDefined(typeof(CRUDModeAttribute)))
            {
                var Attr = Attribute.GetCustomAttribute(ClassType, typeof(CRUDModeAttribute), true) as CRUDModeAttribute;
                AllowedCRUDModes = Attr.Modes;
            }
            else
            {
                AllowedCRUDModes = CRUDMode.All;
            }
        }
        /// <summary>
        /// Constructor.
        /// <para><strong>WARNING:</strong> The specified type should be a subclass of <see cref="DbContext"/> and provide a default constructor.</para>
        /// </summary>
        public EFDataService(Type DataContextType)
            : this()
        {
            if (!DataContextType.IsSubclassOf(typeof(DbContext)))
                throw new Exception("The specified type is not a DbContext subclass.");

            // if it has a default constructor, then it means it handles DbContextOptions itself
            BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public;
            ConstructorInfo Constructor = DataContextType.GetConstructor(BindingFlags, null, new Type[0], null);
            
            if (Constructor == null)
                throw new Exception("The specified type does not provide a default constructor.");

            this.GetDataContextFunc = () => Activator.CreateInstance(DataContextType) as DbContext;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public EFDataService(Func<DbContext> GetDataContextFunc)
        {
            this.GetDataContextFunc = GetDataContextFunc;
        }

        // ● miscs
        /// <summary>
        /// Creates and returns a <see cref="DbContext"/>.
        /// </summary>
        public virtual DbContext GetDataContext()
        {
            DbContext Result = GetDataContextFunc();
            return Result;
        }
        /// <summary>
        /// Executes a specified call-back from insided a transcation.
        /// </summary>
        public virtual void UseTransaction(Action<IDbContextTransaction, DbContext> Proc)
        {
            using (var DataContext = GetDataContext())
            {
                using (var Transaction = DataContext.Database.BeginTransaction())
                {
                    Proc(Transaction, DataContext);
                }
            }
        }

        // ● get entity lists
        /// <summary>
        /// Returns a <see cref="ListResult&lt;T&gt;"/> result list of entities after applying the specified <c>OrderByFunc()</c> and <c>Include()</c> callback functions.
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var Result = MyService.GetListAsync&lt;Product&gt;(q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetListAsync(
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc = null,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetListAsync(DataContext, OrderByFunc, IncludeFuncs);
                    if (Result.List == null || Result.List.Count == 0)
                        Result.NoDataResult();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns a <see cref="ListResult&lt;T&gt;"/> result list of entities after applying the specified <c>FilterFunc</c>, <c>OrderByFunc()</c> and <c>Include()</c> callback functions.
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var Result = MyService.GetListWithFilterAsync&lt;Product&gt;(p => p.Price > 0.5M, q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetListWithFilterAsync(
            Expression<Func<T, bool>> FilterFunc,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetListWithFilterAsync(DataContext, FilterFunc, OrderByFunc, IncludeFuncs);
                    if (Result.List == null || Result.List.Count == 0)
                        Result.NoDataResult();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns <see cref="ListResult&lt;T&gt;"/> result list of entities after applying the specified SQL WHERE filter
        /// and the specified <c>OrderByFunc()</c> and <c>Include()</c> callback functions.
        /// <para><strong>CAUTION: </strong>The filter should be a SQL WHERE section <strong>without</strong> the <c>WHERE</c> keyword.</para>
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var Result = MyService.GetListWithSqlFilterAsync&lt;Product&gt;("Name like '%John%'", q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetListWithSqlFilterAsync(
            string SqlWhereText,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetListWithSqlFilterAsync(DataContext, SqlWhereText, OrderByFunc, IncludeFuncs);
                    if (Result.List == null || Result.List.Count == 0)
                        Result.NoDataResult();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● get entity lists with pagination
        /// <summary>
        /// As the <see cref="GetListAsync()"/>, plus pagination.
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetListPagedAsync(
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResultPaged<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    List<T> List = await GetListPagedAsync(DataContext, Paging, OrderByFunc, IncludeFuncs);
                    if (List == null || List.Count == 0)
                    {
                        Result.NoDataResult();
                    }
                    else
                    {
                        Result.SetFrom(Paging, List);
                    }

 
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// As the <see cref="GetListWithFilterAsync()"/>, plus pagination.
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetListWithFilterPagedAsync(
            IPaging Paging,
            Expression<Func<T, bool>> FilterFunc,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResultPaged<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    List<T> List = await GetListWithFilterPagedAsync(DataContext, Paging, FilterFunc, OrderByFunc, IncludeFuncs);
                    if (List == null || List.Count == 0)
                    {
                        Result.NoDataResult();
                    }
                    else
                    {
                        Result.SetFrom(Paging, List);
                    }
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// As the <see cref="GetListWithSqlFilterAsync()"/>, plus pagination.
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetListWithSqlFilterPagedAsync(
            IPaging Paging,
            string SqlWhereText,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs)
        {
            ListResultPaged<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    List<T> List = await GetListWithSqlFilterPagedAsync(DataContext, Paging, SqlWhereText, OrderByFunc, IncludeFuncs);
                    if (List == null || List.Count == 0)
                    {
                        Result.NoDataResult();
                    }
                    else
                    {
                        Result.SetFrom(Paging, List);
                    }
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
 

        // ● get single entity
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified primary key.
        /// </summary>
        public virtual async Task<ItemResult<T>> GetByIdAsync(string Id)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetById);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByIdAsync(DataContext, Id);
                    if (Result.Item == null)
                        Result.ErrorResult(ApiStatusCodes.EntityNotFound);
                }

            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// </summary>
        public virtual async Task<ItemResult<T>> GetByProcAsync(Func<T, bool> Proc)
        {
            ItemResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByProcAsync(DataContext, Proc);
                    if (Result.Item == null)
                        Result.ErrorResult(ApiStatusCodes.EntityNotFound);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> InsertAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Insert);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = Insert(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Updates an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> UpdateAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Update);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = Update(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Deletes an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> DeleteAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Delete);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = Delete(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Deletes an entity under a specified Id, if any, else returns a failed result.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> DeleteAsync(string Id)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Delete);

                using (var DataContext = GetDataContext())
                {
                    if (string.IsNullOrWhiteSpace(Id))
                    {
                        Result.ErrorResult(ApiStatusCodes.EntityIdNotSpecified);
                    }
                    else
                    {
                        T Entity = await GetByIdAsync(DataContext, Id);
                        if (Entity == null)
                        {
                            Result.ErrorResult(ApiStatusCodes.EntityNotFound, $"{typeof(T).Name} Entity not found by Id: {Id}");
                        }
                        else
                        {
                            Result.Item = Delete(DataContext, Entity);
                            await DataContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● properties
        /// <summary>
        /// Returns the table name of the entity.
        /// </summary>
        public string TableName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fTableName))
                {
                    using (var DataContext = GetDataContext())
                    {
                        DbSet<T> DbSet = DataContext.Set<T>();
                        fTableName = DbSet.EntityType.GetTableName();

                        // or
                        //IEntityType EntityType = DataContext.Model.FindEntityType(typeof(T));
                        //if (EntityType != null)
                        //    fTableName = EntityType.GetTableName();

                        if (string.IsNullOrWhiteSpace(fTableName))
                        {
                            Type ClassType = typeof(T);

                            if (ClassType.IsDefined(typeof(TableAttribute)))
                            {
                                var Attr = Attribute.GetCustomAttribute(ClassType, typeof(TableAttribute), true) as TableAttribute;
                                fTableName = Attr.Name;
                            }

                            if (string.IsNullOrWhiteSpace(fTableName))
                                fTableName = ClassType.Name;
                        }

                    }
                }

                return fTableName;
            }
        }
        /// <summary>
        /// A bit-field indicating the CRUD operations allowed to this Entity
        /// </summary>
        public CRUDMode AllowedCRUDModes { get; }


    }

 
}
