using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userList = await _userLoginService.GetAll();
            return Ok(userList);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(AuthenticateRequest authenticateRequest)
        {
            var token = await _userLoginService.Login(authenticateRequest);
            return Ok(token);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserLoginDto newUser)
        {
            var result = await _userLoginService.Register(newUser);
            return Ok(result);
        }
    }
}
