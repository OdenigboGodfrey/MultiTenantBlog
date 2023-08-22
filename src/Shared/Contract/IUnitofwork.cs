using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenantBlogTest.src.Shared.Contract
{
    public interface IUnitofwork
    {
        public Task CommitAsync();
        // public IUserServices Userservice { get; }
    }
}
