using Application.Common.BranchDTOS;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBranchService : IGenericService<Branch, GetBranchDTO, object, AddBranchDTO, UpdateBranchDTO>
    {
        Task<List<BranchLookUp>> GetLookup();

        //Task<List<GetBranchDTO>> GetAllAsync(CancellationToken ct);
        //Task<GetBranchDTO?> GetByIdAsync(int id, CancellationToken ct);
        //Task CreateAsync(GetBranchDTO dto, CancellationToken ct);
        //Task UpdateAsync(GetBranchDTO dto, CancellationToken ct);
        //Task DeleteAsync(int id, CancellationToken ct);
        //Task<PagedResponse<GetBranchDTO>> GetPagedAsync(PagingDTO paging, CancellationToken ct);

    }
}
