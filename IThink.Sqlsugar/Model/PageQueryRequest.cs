namespace IThink.Sqlsugar
{

    /// <summary>
    /// 分页查询模型
    /// </summary>
    public class PageQueryRequest
    {
        /// <summary>
        /// 分页大小
        /// </summary>
        private int _pageSize = 20;
        public int PageSize
        {
            get { return this._pageSize; }
            set
            {
                if (value != 0)
                {
                    this._pageSize = value;
                }
            }
        }

        private int _pageNumber = 1;

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber
        {
            get { return this._pageNumber; }
            set
            {
                if (value != 0)
                {
                    this._pageNumber = value;
                }
            }
        }
    }

    /// <summary>
    /// 分页查询模型
    /// </summary>
    public class PageQueryRequest<T> : PageQueryRequest
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        public T QueryData { get; set; }
    }
}

