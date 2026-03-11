namespace Application.Common.Pagination
{
    public class FilteredPagingDTO : PagingDTO
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}
