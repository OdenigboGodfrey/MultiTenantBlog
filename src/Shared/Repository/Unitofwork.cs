using Microsoft.AspNetCore.Http;
using MultiTenantBlogTest.src.Shared.Contract;
using MultiTenantBlogTest.src.Shared.DatabaseContext;
using MultiTenantBlogTest.src.Tenant.Contract;
using MultiTenantBlogTest.src.Tenant.SchemaTenant;
using MultiTenantBlogTest.src.Tenant.SchemaTenant.SchemaContext;
using MultiTenantBlogTest.src.Tenant.Service;
using System.Threading.Tasks;
using MultiTenantBlogTest.src.User.Contract;
using MultiTenantBlogTest.src.User.Services;

namespace MultiTenantBlogTest.src.Shared.Repository
{
    public class Unitofwork : IUnitofwork
    {

        private readonly ApplicationDbContext subdomainSchemaContext;
        private readonly ApplicationDbContext publicSchemaContext;
        private readonly ITenantSchema tenantSchema;
        private ITenantService GetTenantService;
        
        private IUserServices GetUserServices;
        private IHttpContextAccessor _httpContextAccessor;
        
        public Unitofwork(IHttpContextAccessor httpContextAccessor, ITenantConfig<ApplicationDbContext> tenantConfig, ITenantSchema tenantSchema)
        {
            var _host = tenantSchema.ExtractSubdomainFromRequest(httpContextAccessor.HttpContext);

            this.subdomainSchemaContext = tenantConfig.getRequestContext(_host);
            _httpContextAccessor = httpContextAccessor;
            publicSchemaContext = new ApplicationDbContext(new DbContextSchema());
            this.tenantSchema = tenantSchema;
        }

        public ITenantService TenantService
        {
            get
            {
                if (this.GetTenantService == null) { this.GetTenantService = new TenantService(this.publicSchemaContext, this.subdomainSchemaContext, this.tenantSchema); }
                return this.GetTenantService;
            }
        }

        public IUserServices UserServices {
            get
            {
                if (this.GetUserServices == null) {
                    this.GetUserServices = new UserServices(this.publicSchemaContext, this._httpContextAccessor, this.subdomainSchemaContext);
                }
                return this.GetUserServices;
            }
        }
    }
}
