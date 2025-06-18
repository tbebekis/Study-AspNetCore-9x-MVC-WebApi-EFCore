namespace CommonLib
{
    static public class EFExtensions
    {
        // ● DbContext
        /// <summary>
        /// Returns the <see cref="IQueryable&lt;T&gt;"/> of a specified type parameter.
        /// <para>Example call</para>
        /// <code>
        ///     var Q = MyContext.GetQuery&lt;Product&gt;();
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public IQueryable<T> GetQuery<T>(this DbContext DataContext) where T : class
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            var Q = DbSet.AsNoTracking();
            return Q;
        }
        /// <summary>
        /// Returns the <see cref="IQueryable&lt;T&gt;"/> of a specified type parameter after applying a filter callback function.
        /// <para>Example call</para>
        /// <code>
        ///     var Q = MyContext.ApplyFilter&lt;Product&gt;(p => p.Price > 0.5M);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public IQueryable<T> ApplyFilter<T>(this DbContext DataContext, Expression<Func<T, bool>> FilterFunc) where T : class
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            var Q = DbSet.Where(FilterFunc).AsNoTracking();
            return Q;
        }
        /// <summary>
        /// Returns the <see cref="IQueryable&lt;T&gt;"/> of a specified type parameter after applying a SQL WHERE filter.
        /// <para><strong>CAUTION: </strong>The filter should be a SQL WHERE section <strong>without</strong> the <c>WHERE</c> keyword.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var Q = MyContext.ApplySqlFilter&lt;Product&gt;("Name like '%John%'");
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public IQueryable<T> ApplySqlFilter<T>(this DbContext DataContext, string SqlWhereText) where T : class
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            string TableName = DbSet.EntityType.GetTableName();
            string SqlText = $"select * from {TableName} where " + SqlWhereText;

            var Q = DbSet.FromSqlRaw(SqlText).AsNoTracking();
            return Q;
        }


        // ● IQueryable<T>
        /// <summary>
        /// Returns a <see cref="List&lt;T&gt;"/> list of entities after applying the specified <c>OrderBy()</c> and <c>Include()</c> callback functions.
        /// <para><strong>NOTE:</strong> The result is paginated if a <see cref="IPaging"/> instance is passed.</para>
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var List = Q.Apply&lt;Product&gt;(new Paging(2, 5), q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// </summary>
        static public async Task<List<T>> Apply<T>(this IQueryable<T> Q,
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs
            ) where T : class
        {
            if (OrderByFunc != null)
                Q = OrderByFunc(Q);

            if (Paging != null)
            {
                Paging.TotalItems = await Q.CountAsync();

                Q = Q.Skip(Paging.PageIndex * Paging.PageSize)
                    .Take(Paging.PageSize);
            }

            foreach (Expression<Func<T, object>> IncludeFunc in IncludeFuncs)
                Q = Q.Include(IncludeFunc);

            return await Q.ToListAsync();
        }


        // ● get list variations
        /// <summary>
        /// Returns a <see cref="List&lt;T&gt;"/> list of entities after applying the specified <c>OrderBy()</c> and <c>Include()</c> callback functions.
        /// <para><strong>NOTE:</strong> The result is paginated if a <see cref="IPaging"/> instance is passed.</para>
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var List = MyContext.GetList&lt;Product&gt;(new Paging(2, 5), q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public async Task<List<T>> GetList<T>(this DbContext DataContext,
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs
            ) where T : class
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            var Q = DbSet.AsNoTracking(); 

            return await Q.Apply(Paging, OrderByFunc, IncludeFuncs);
        }
        /// <summary>
        /// Returns a <see cref="List&lt;T&gt;"/> list of entities after applying the specified <c>Where()</c>, <c>OrderBy()</c> and <c>Include()</c> callback functions.
        /// <para><strong>NOTE:</strong> The result is paginated if a <see cref="IPaging"/> instance is passed.</para>
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var List = MyContext.GetListWithFilter&lt;Product&gt;(p => p.Price > 0.5M, new Paging(2, 5), q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public async Task<List<T>> GetListWithFilter<T>(this DbContext DataContext,
            Expression<Func<T, bool>> FilterFunc,
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs
            ) where T : class
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            var Q = DbSet.AsNoTracking();

            if (FilterFunc != null)
                Q = Q.Where(FilterFunc);

            return await Q.Apply(Paging, OrderByFunc, IncludeFuncs);
        }
        /// <summary>
        /// Returns a <see cref="List&lt;T&gt;"/> list of entities after applying the specified SQL WHERE filter
        /// and the specified <c>OrderBy()</c> and <c>Include()</c> callback functions.
        /// <para><strong>CAUTION: </strong>The filter should be a SQL WHERE section <strong>without</strong> the <c>WHERE</c> keyword.</para>
        /// <para><strong>NOTE:</strong> The result is paginated if a <see cref="IPaging"/> instance is passed.</para>
        /// <para><strong>NOTE:</strong> Any of the parameteres could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        ///     var List = MyContext.GetListWithSqlFilter&lt;Product&gt;("Name like '%John%'", new Paging(2, 5), q => q.OrderBy(p => p.Name), p => p.Category);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public async Task<List<T>> GetListWithSqlFilter<T>(this DbContext DataContext,
            string SqlWhereText,
            IPaging Paging,
            Func<IQueryable<T>, IQueryable<T>> OrderByFunc,            
            params Expression<Func<T, object>>[] IncludeFuncs
            ) where T : class
        {

            DbSet<T> DbSet = DataContext.Set<T>();
            string TableName = DbSet.EntityType.GetTableName();

            string SqlText = $"select * from {TableName} where " + SqlWhereText;

            var Q = DbSet.FromSqlRaw(SqlText).AsNoTracking();

            return await Q.Apply(Paging, OrderByFunc, IncludeFuncs);
        }


        // ● get list with EFListParams
        /// <summary>
        /// Returns a <see cref="List&lt;T&gt;"/> list of entities after applying the specified <see cref="EFListParams{T}"/> properties.
        /// <para><strong>NOTE:</strong> The result is paginated if the <see cref="EFListParams{T}.Paging"/> is not null.</para>
        /// <para><strong>NOTE:</strong> Any of the <see cref="EFListParams{T}"/> properties could be specified as null.</para>
        /// <para>Example call</para>
        /// <code>
        /// 
        /// EFListParams&lt;Product&gt; Params = new EFListParams&lt;Product&gt;(p => p.Category)  
        /// {
        ///     Paging = new Paging(1, 5),                    
        /// };
        /// 
        /// // or
        /// // Params.IncludeFuncs.Add(p => p.Category);
        /// 
        /// var Result = await DataContext.GetList&lt;Product&gt;(Params);
        /// </code>
        /// <para><strong>NOTE: </strong> No change tracking.</para>
        /// </summary>
        static public async Task<List<T>> GetList<T>(this DbContext DataContext, EFListParams<T> Params) where T : BaseEntity
        {
            IQueryable<T> Q = null;
            if (Params.FilterFunc != null)
                Q = DataContext.ApplyFilter<T>(Params.FilterFunc);
            else if (!string.IsNullOrWhiteSpace(Params.SqlWhereText))
                Q = DataContext.ApplySqlFilter<T>(Params.SqlWhereText);
            else
            {
                DbSet<T> DbSet = DataContext.Set<T>();
                Q = DbSet.AsNoTracking();
            }

            if (Params.OrderByFunc != null)
                Q = Params.OrderByFunc(Q);

            if (Params.Paging != null)
            {
                Params.Paging.TotalItems = await Q.CountAsync();

                Q = Q.Skip(Params.Paging.PageIndex * Params.Paging.PageSize)
                    .Take(Params.Paging.PageSize);
            }

            if (Params.IncludeFuncs != null)
                foreach (Expression<Func<T, object>> IncludeFunc in Params.IncludeFuncs)
                    Q = Q.Include(IncludeFunc);

            return await Q.ToListAsync();

        }
    }
}
