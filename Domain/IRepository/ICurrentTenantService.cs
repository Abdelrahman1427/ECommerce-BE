namespace Domain.IRepository
{
    public interface ICurrentTenantService
    {
        int? BranchId { get; }
        string UserName { get; }
    }
}
