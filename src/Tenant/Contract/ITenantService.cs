using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiTenantBlogTest.src.Shared.ViewModels;
using MultiTenantBlogTest.src.Tenant.ViewModel;

namespace MultiTenantBlogTest.src.Tenant.Contract
{
    public interface ITenantService
    {
        public Task<TenantModelVM> GetTenantById(Guid id);
        public Task<ApiResponse<bool>> RegisterTenant(RegisterTenantVM model);
        Task<TenantMigrationResultVM> MigrateTenants();
        // public UserCompanies AddUserToCompany(string userId, Guid tenantId);
        // public TenantModelVM GetUserTenant(string userId);
        Task<ApiResponse<bool>> DeleteTenant(string subdomain);
        Task<TenantModelVM> GetTenantBySubdomain(string subdomain);
        // Task<ApiResponse<List<UserCompaniesVM>>> GetTenantUsers (string subdomain);
    }
}
