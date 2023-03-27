using AuthenticationService.Bussiness.common;
using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.DTOs;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpPost]
        [Route("GetAll")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authenService.GetAll();
            return Ok(result);
        }

        [HttpPost]
        [Route("AuthenGetAll")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AuthenGetAll()
        {
            var result = await _authenService.GetAll();
            return Ok(result);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return BadRequest();

            var result = await _authenService.Register(registerRequest);
            return Ok(result);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {

            if (loginModel == null)
                return BadRequest();

            var result = await _authenService.Login(loginModel);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {


            var result = await _authenService.Logout();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
                return BadRequest();

            var result = await _authenService.RefreshToken(tokenModel);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] string username)
        {

            if (username == null || username == "")
                return BadRequest();

            await _authenService.RevokeToken(username);
            return NoContent(); 
        }



    }
}
