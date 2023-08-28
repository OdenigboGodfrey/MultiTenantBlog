using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MultiTenantBlogTest.src.Shared.ViewModels;
using MultiTenantBlogTest.src.User.Models;

namespace MultiTenantBlogTest.src.User.Contract
{
    public interface IUserServices
    {

        //public Task<List<User>> GetAllUsers(Region region = Region.Africa);
        public Task<Page<UserModel>> GetAllUsers(int pageNumber = 1, int pageSize = 20, string searchparam = null);
        public Task<UserModel> GetUserById(string ID);
        public Task<bool> SaveUser(UserModel user);
        public Task<bool> SaveNewUserProfile(UserModel user);
        public Task<bool> SaveChangesAsync();
        public Task<UserModel> GetUserByEmail(string email);
    }
}
