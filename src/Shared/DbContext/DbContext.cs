using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MultiTenantBlogTest.src.Tenant.SchemaTenant.SchemaContext;
using MultiTenantBlogTest.src.Shared.Models;

namespace MultiTenantBlogTest.src.Shared.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<User>, IDbContextSchema
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MultiTenantBlog"];
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                       IDbContextSchema schema = null) : base(options)
        {
            // used by dotnet ef during migration
            Schema = schema?.Schema;
            _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MultiTenantBlog"];
        }
        public ApplicationDbContext(string connectionString, IDbContextSchema schema = null)
        {
            Schema = schema?.Schema;
            _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MultiTenantBlog"];
        }
        public ApplicationDbContext(IDbContextSchema schema = null)
        {
            Schema = schema?.Schema;
            _connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MultiTenantBlog"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(3300);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schema);
                })
                .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
                .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>();

            base.OnConfiguring(optionsBuilder);
        }

        public string Schema { get; set; } = "dbo";
        private readonly string _connectionString;

        // public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Tenant.Model.Tenant> Tenants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            base.OnModelCreating(modelBuilder);
        }
    }
}
