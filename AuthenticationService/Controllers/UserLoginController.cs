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

        [HttpPost]
        [Route("GetTimeZone")]
        public async Task<IActionResult> GetTimeZone()
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            var a = localZone.ToLocalTime;
            var b = localZone.ToUniversalTime;
            DateTime currentDate = DateTime.Now;

            DateTime currentUTC = localZone.ToUniversalTime(currentDate);
            TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

            string currentTimeZone = $@"
                                        CurrentUTC: {currentUTC}
                                        UTC offset: {currentOffset}
                                        Standard time name: {localZone.StandardName}
                                        Daylight saving time name: {localZone.DaylightName}
                                        Current date and time: {DateTime.Now}
                                        
";

            //var token = await _userLoginService.GetUserProfile();
            return Ok(currentTimeZone);
        }

    }
}
