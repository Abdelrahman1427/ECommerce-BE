using Domain.IRepository;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Repositories
{
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? BranchId
        {
            get
            {
                var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst("BranchId")?.Value;
                return !string.IsNullOrEmpty(claimValue) ? int.Parse(claimValue) : (int?)null;
            }
        }

        public string UserName =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
    }

}
