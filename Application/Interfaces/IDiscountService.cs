using Application.Common.DiscountDTOS;
using Application.Common.Pagination;

namespace Application.Interfaces
{
    public interface IDiscountService
    {
        Task<PagedResponse<DiscountDto>> GetAllAsync(PagingDTO paging);
        Task<DiscountDto?> GetByCodeAsync(string code);
        Task<DiscountDto> CreateAsync(AddDiscountDTO dto);
        Task<DiscountDto> UpdateAsync(int id, UpdateDiscountDTO dto);
        Task DeleteAsync(int id);
        Task<ApplyDiscountResultDto> ApplyAsync(int orderId, string code, string userId);
    }
}
