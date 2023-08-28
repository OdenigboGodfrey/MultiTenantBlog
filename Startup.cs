using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MultiTenantBlogTest.src.Shared.DatabaseContext;
using MultiTenantBlogTest.src.Tenant.SchemaTenant.SchemaContext;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MultiTenantBlogTest.src.Shared.Repository;
using MultiTenantBlogTest.src.Shared.Contract;
using Microsoft.AspNetCore.Http;
using MultiTenantBlogTest.src.Tenant.Service;
using MultiTenantBlogTest.src.Tenant.SchemaTenant;
using MultiTenantBlogTest.src.User.Models;
using Microsoft.AspNetCore.Identity;

namespace MultiTenantBlogTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            /**
            1) Schema Modification: Make Context Schema Aware
            */
            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(
                    Configuration.GetConnectionString("MultiTenantBlog"))
                    .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>()
                    .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
                    ).AddSingleton<IDbContextSchema>(new DbContextSchema());

            services.AddScoped<IUnitofwork, Unitofwork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
            //////////////////////////Enable  Cross ORigin//////////////////////////////////////////

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("*")
                        .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            /**
                2) Schema Modification: Add Tenant Control Pipeline (in charge of changing context schema based on subdomain etc)
            */
            services.AddScoped<ITenantConfig<ApplicationDbContext>, TenantConfig>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ITenantSchema, TenantSchema>();
            services.AddTransient<TenantService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MultiTenantBlogTest", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TenantService tenantService)
        {
            app.CustomEnginerInterceptor();
            // run migrations
            tenantService.MigrateTenants();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MultiTenantBlogTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
