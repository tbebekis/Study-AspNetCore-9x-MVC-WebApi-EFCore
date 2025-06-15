namespace StudyLib
{

    /// <summary>
    /// Used in passing parameters to the <see cref="EFExtensions.GetList{T}(DbContext, EFListParams{T})"/> extension method.
    /// </summary>
    public class EFListParams<T> where T : BaseEntity
    {
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public EFListParams()
        {
        }
        /// <summary>
        /// Constructor.
        /// <para>The caller may specified one or more <c>Include()</c> callback functions.</para>
        /// <para>Example call</para>
        /// <code>
        /// var Params = new EFListParams&lt;Product&gt;(p => p.Category)
        /// </code>
        /// </summary>
        public EFListParams(params Expression<Func<T, object>>[] IncludeFuncs) 
        {
            this.IncludeFuncs.AddRange(IncludeFuncs);
        }
        /// <summary>
        /// Constructor.
        /// <para>The caller should specify a filter callback function.</para>
        /// <para>Example call</para>
        /// <code>
        /// var Params = new EFListParams&lt;Product&gt;(p => p.Price > 0.5M);
        /// </code>
        /// </summary>
        public EFListParams(Expression<Func<T, bool>> FilterFunc)
        {
            this.FilterFunc = FilterFunc;
        }
        /// <summary>
        /// Constructor.
        /// <para>The caller should specify a SQL WHERE filter.</para>
        /// <para><strong>CAUTION: </strong>The filter should be a SQL WHERE section <strong>without</strong> the <c>WHERE</c> keyword.</para>
        /// <para>Example call</para>
        /// <code>
        /// var Params = new EFListParams&lt;Product&gt;("Name like '%John%'");
        /// </code>
        /// </summary>
        public EFListParams(string SqlWhereText)
        {
            this.SqlWhereText = SqlWhereText;
        }

        // ● properties
        /// <summary>
        /// A filter callback function such as
        /// <code>p => p.Price > 0.5M</code>
        /// </summary>
        public Expression<Func<T, bool>> FilterFunc { get; set; }
        /// <summary>
        /// A SQL WHERE filter text as
        /// <code>"Name like '%John%'"</code>
        /// <para><strong>CAUTION: </strong>The filter should be a SQL WHERE section <strong>without</strong> the <c>WHERE</c> keyword.</para>
        /// </summary>
        public string SqlWhereText { get; set; }
        /// <summary>
        /// A filter callback function such as
        /// <code>p => p.Price > 0.5M</code>
        /// </summary>
        public Func<IQueryable<T>, IQueryable<T>> OrderByFunc { get; set; }
        /// <summary>
        /// A <see cref="Paging"/> instance as
        /// <code>new Paging(2, 5)</code>
        /// </summary>
        public IPaging Paging { get; set; }
        /// <summary>
        /// A list of <c>Include()</c> callback functions, as
        /// <code>IncludeFuncs.Add(p => p.Category)</code>
        /// </summary>
        public List<Expression<Func<T, object>>> IncludeFuncs { get; set; } = new List<Expression<Func<T, object>>>();
    }
}
