using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Jobid.App.Helpers.Models;

namespace MultiTenantBlogTest.src.Shared.ViewModels
{
    public class ApiResponse<T>
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        // public User user { get; set; }
        public T Data { get; set; }

    }
}
