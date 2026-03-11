using Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Helper
{
    public static class PaginationHelper
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        PagingDTO pagingDto)
        {
            var pageNumber = pagingDto.PageNumber < 1 ? 1 : pagingDto.PageNumber;
            var pageSize = pagingDto.PageSize < 1 ? 10 : pagingDto.PageSize;

            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new PagedResponse<T>(data, totalCount, pagingDto.PageNumber, pagingDto.PageSize);
        }
    }
}
