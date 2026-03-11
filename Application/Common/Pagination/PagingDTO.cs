namespace Application.Common.Pagination
{
    public class PagingDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
    public class PagingDTO<TFilter> : PagingDTO
    {
        public TFilter? Filter { get; set; }
    }
}
