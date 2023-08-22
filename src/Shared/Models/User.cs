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
    public class User : IdentityUser
    {
        public DateTime Created_At { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Status { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

