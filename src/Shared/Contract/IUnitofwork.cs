using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiTenantBlogTest.src.User.Contract;

namespace MultiTenantBlogTest.src.Shared.Contract
{
    public interface IUnitofwork
    {
        public IUserServices UserServices { get; }
    }
}
