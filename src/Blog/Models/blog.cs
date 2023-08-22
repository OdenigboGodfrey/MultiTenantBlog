using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MultiTenantBlogTest.src.Shared.Models
{
    public class Blog
    {
        public DateTime Created_At { get; set; }

        public string title { get; set; }

        public string content { get; set; }
        public string parentId { get; set; }
    }
}

