using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MultiTenantBlogTest.src.Shared.Contract;
using MultiTenantBlogTest.src.Shared.DbContext;
using MultiTenantBlogTest.src.Shared.Models;
using MultiTenantBlogTest.src.Tenant;
using MultiTenantBlogTest.src.Tenant.Contract;
using MultiTenantBlogTest.src.Tenant.SchemaTenant;
using MultiTenantBlogTest.src.Tenant.SchemaTenant.SchemaContext;
using MultiTenantBlogTest.src.Tenant.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenantBlogTest.src.Shared.Repository
{
    public class Unitofwork : IUnitofwork
    {

        private readonly ApplicationDbContext DbCon;
        private readonly ApplicationDbContext publicSchemaContext;
        private readonly UserStore<User> userStore;
        private readonly UserManager<User> userManager;
        private readonly ITenantSchema tenantSchema;
        private ITenantService GetTenantService;
        
        // private IUserServices GetUserServices;
        private IHttpContextAccessor _httpContextAccessor;
        
        public Unitofwork(IHttpContextAccessor httpContextAccessor, ITenantConfig<ApplicationDbContext> tenantConfig, ITenantSchema tenantSchema)
        {
            var _host = tenantSchema.ExtractSubdomainFromRequest(httpContextAccessor.HttpContext);

            this.DbCon = tenantConfig.getRequestContext(_host);
            _httpContextAccessor = httpContextAccessor;
            publicSchemaContext = new ApplicationDbContext(new DbContextSchema());
            userStore = new UserStore<User>(publicSchemaContext);
            userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            this.tenantSchema = tenantSchema;
        }


        public async Task CommitAsync()
        {

            await this.publicSchemaContext.SaveChangesAsync();

        }

        public ITenantService TenantService
        {
            get
            {
                if (this.GetTenantService == null) { this.GetTenantService = new TenantService(this.publicSchemaContext, this.userManager, this.DbCon, this.tenantSchema); }
                return this.GetTenantService;
            }
        }
    }
}
