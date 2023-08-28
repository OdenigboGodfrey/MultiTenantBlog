using System;

namespace MultiTenantBlogTest.src.Blog.Models
{
    public class Blog
    {
        public DateTime Created_At { get; set; }

        public string title { get; set; }

        public string content { get; set; }
        public string parentId { get; set; }
        public string userId { get; set; } = "TestUserId";
    }
}

