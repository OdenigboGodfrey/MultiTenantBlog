using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using MultiTenantBlogTest.src.Shared.ViewModels;

namespace MultiTenantBlogTest.src.Tenant.SchemaTenant
{
    public static class Extensions
    {
        public static IApplicationBuilder CustomEnginerInterceptor(this IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                TenantSchema tenantSchema = new TenantSchema();
                var origin = tenantSchema.ExtractSubdomainFromRequest(context);
                tenantSchema._schema = origin;
                var schemaExists = await tenantSchema.DoesCurrentSubdomainExist();
                Console.WriteLine($"context.Request.Headers {context.Request.Headers["Referer"]}, origin {origin}");

                if (!schemaExists)
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    ApiResponse<dynamic> apiResponse = new ApiResponse<dynamic>
                    {
                        Data = null,
                        ResponseCode = "404",
                        ResponseMessage = "Not found",
                        // user = null,
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
                    return;
                }
                // check schema if not exist on every request
                await next.Invoke();
            });
            return builder;
        }

        public static object ExecuteScalar(this DbContext context, string sql,
       List<DbParameter> parameters = null,
       CommandType commandType = CommandType.Text,
       int? commandTimeOutInSeconds = null)
        {
            Object value = ExecuteScalar(context.Database, sql, parameters,
                                         commandType, commandTimeOutInSeconds);
            return value;
        }

        public static object ExecuteScalar(this DatabaseFacade database,
        string sql, List<DbParameter> parameters = null,
        CommandType commandType = CommandType.Text,
        int? commandTimeOutInSeconds = null)
        {
            Object value;
            using (var cmd = database.GetDbConnection().CreateCommand())
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = sql;
                cmd.CommandType = commandType;
                if (commandTimeOutInSeconds != null)
                {
                    cmd.CommandTimeout = (int)commandTimeOutInSeconds;
                }
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                value = cmd.ExecuteScalar();
            }
            return value;
        }

    }
}