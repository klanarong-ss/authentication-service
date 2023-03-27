using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        public UserLoginController(IUserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        [HttpPost]
        [Route("GetUserProfile")]
        public async Task<IActionResult> Login()
        {
            var token = await _userLoginService.GetUserProfile();
            return Ok(token);
        }

    }
}
