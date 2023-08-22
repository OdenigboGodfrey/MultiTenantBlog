using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MultiTenantBlogTest.src.Shared.DbContext;
using MultiTenantBlogTest.src.Tenant.SchemaTenant.SchemaContext;
using MultiTenantBlogTest.src.Shared.Utilities;

namespace MultiTenantBlogTest.src.Tenant.SchemaTenant
{

    public interface ITenantSchema
    {
        ApplicationDbContext getRequestContext();
        ApplicationDbContext getRequestContext(string hostURL);
        Task<bool> DoesCurrentSubdomainExist();
        Task<bool> RunMigrations(string schemaName);
        Task<bool> NewSchema(string schemaName);
        string ExtractSubdomainFromRequest(HttpContext httpContext);
    }

    public class TenantSchema : ITenantSchema
    {
        public string _schema;
        private string conString;

        public ApplicationDbContext context;
        private readonly IConfiguration _config;


        public TenantSchema()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            conString = _config.GetSection("ConnectionStrings")["MultiTenantBlog"];
            this.context = new ApplicationDbContext(conString, new DbContextSchema());

        }
        public TenantSchema(string hostURL)
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            // open new connection
            conString = _config.GetSection("ConnectionStrings")["MultiTenantBlog"];
            this.context = new ApplicationDbContext(conString, new DbContextSchema());
            // create new schema if not exist
            _schema = getSubdomainName(hostURL);
        }



        public async Task<bool> NewSchema(string schemaName)
        {
            schemaName = Utility.prepareSubdomainName(schemaName);

            var _schemaExist = (string)context.ExecuteScalar($"SELECT name FROM sys.schemas where name = '{schemaName}';");
            if (string.IsNullOrEmpty(_schemaExist))
            {
                // doesnt exist
                // create 
                await this.context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA {schemaName};");
                _schemaExist = (string)context.ExecuteScalar($"SELECT name FROM sys.schemas where name = @paramName;", new List<DbParameter>() { new Microsoft.Data.SqlClient.SqlParameter("@paramName", schemaName) });
            }

            return string.IsNullOrEmpty(_schemaExist) ? false : true;
        }

        private async Task<int> ExecSQL(string sql)
        {
            return (int)await this.context.Database.ExecuteSqlRawAsync(sql);
        }

        private T ExecScalar<T>(string sql, List<DbParameter> parameters)
        {
            return (T)context.ExecuteScalar(sql, parameters);
        }

        public string getSubdomainName(string domainURL)
        {
            string domainName = domainURL;
            domainName = domainName.Replace("http://", "").Replace("https://", "");
            
            domainName = domainName.Split(":")[0];
            var _schemaName = domainName.Split(".")[0];
            _schemaName = Utility.prepareSubdomainName(_schemaName);
            return _schemaName;
        }

        public async Task<bool> RunMigrations(string schemaName)
        {
            try
            {
                schemaName = Utility.prepareSubdomainName(schemaName);
                var schema = await this.NewSchema(schemaName);
                if (!schema) throw new Exception("Schema not created.");
                this.context = new ApplicationDbContext(conString, new DbContextSchema(schemaName));
                // check if pending migrations first
                this.context.Database.Migrate();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public ApplicationDbContext getRequestContext()
        {
            return this.context;
        }

        public ApplicationDbContext getRequestContext(string hostURL)
        {
            return new ApplicationDbContext(conString, new DbContextSchema(getSubdomainName(hostURL)));
        }

        public async Task<bool> DoesCurrentSubdomainExist()
        {
            string rootDomain = _config["RootDomain"];
            var tenant = await this.context.Tenants.FirstOrDefaultAsync(x => x.Subdomain == _schema);
            if (_schema == "api" || _schema == "admin" || _schema == rootDomain)
            {
                return true;
            }
            if (tenant == null) return false;
            Console.WriteLine($"tenant ${tenant.Subdomain}");
            return true;
        }

        public string ExtractSubdomainFromRequest(HttpContext httpContext)
        {

            var _host = httpContext.Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(_host))
            {
                _host = httpContext.Request.Host.ToString();
            }

            _host = getSubdomainName(_host);

            return _host;
        }

    }

}