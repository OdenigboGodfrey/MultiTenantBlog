using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiTenantBlogTest.src.Shared.Contract;
using MultiTenantBlogTest.src.User.Models;
using NLog;
using MultiTenantBlogTest.src.Tenant.SchemaTenant;
using MultiTenantBlogTest.src.Shared.ViewModels;
using MultiTenantBlogTest.src.User.ViewModels;
using MultiTenantBlogTest.src.Shared.Utilities;

namespace MultiTenantBlogTest.src.User.Controllers
{
    [Route("api/[controller]")]
    [Produces("Application/json")]
    [ApiController]

    public class IdentityController : ControllerBase
    {

        private readonly IUnitofwork Services_Repo;
        public readonly IConfiguration config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ILogger Log = LogManager.GetLogger("IdentityController");
        private ITenantSchema tenantSchema;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public IdentityController(IUnitofwork unitofwork, IConfiguration config, IHttpContextAccessor httpContextAccessor, ITenantSchema tenantSchema, IPasswordHasher<UserModel> _passwordHasher)

        {
            this.Services_Repo = unitofwork;
            this.config = config;
            _httpContextAccessor = httpContextAccessor;
            this.tenantSchema = tenantSchema;
            this._passwordHasher = _passwordHasher;

        }
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(
           [FromQuery] int pageSize = 20,
           [FromQuery] int pageNumber = 1,
           [FromQuery] string search = null)
        {
            try
            {
                var result = await this.Services_Repo.UserServices.GetAllUsers(pageNumber, pageSize, search);
                return Ok(new ApiResponse<Page<UserModel>>
                {
                    ResponseMessage = "Successful",
                    ResponseCode = "00",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<Page<UserModel>>
                {
                    ResponseMessage = ex.Message,
                    ResponseCode = "500",
                    Data = null
                });
            }

        }

        [HttpPost]
        [Route("UserSignup")]
        public async Task<IActionResult> UserSignup([FromBody] UserSignupModel Model)
        {

            var User = await this.Services_Repo.UserServices.GetUserByEmail(Model.Email);
            if (User == null)
            {

                UserModel NewUser = new UserModel
                {
                    Email = Model.Email,
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    Created_At = DateTime.Now,
                    Status = "Active",
                    Password = _passwordHasher.HashPassword(null, Model.Password)
                };

                var Result = await this.Services_Repo.UserServices.SaveUser(NewUser);

                Log.Info("Create user response " + JsonConvert.SerializeObject(Result));
                if (Result)
                {
                    var user = this.Services_Repo.UserServices.GetUserByEmail(Model.Email);
                    return Ok(new ApiResponse<Boolean>
                    {
                        ResponseCode = "00",
                        ResponseMessage = "User creation was successful",
                        Data = Result,
                        user = user.Result
                    });

                }
                else
                {
                    return BadRequest(new ApiResponse<Boolean>
                    {
                        ResponseCode = "400",
                        ResponseMessage = "User creation failed",
                        Data = Result
                    });
                }
            }
            else
            {
                return BadRequest(new ApiResponse<Boolean>
                {
                    ResponseCode = "409",
                    ResponseMessage = "Email Already Exist",
                    Data = false
                });
            }
        }
    }
}