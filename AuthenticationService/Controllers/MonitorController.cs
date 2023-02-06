using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        public MonitorController()
        {

        }

        [HttpGet]
        [Route("monitor1")]
        public IActionResult monitor1()
        {
            return Ok("monitor-1");
        }

        [HttpGet]
        [Route("monitor2")]
        public IActionResult monitor2()
        {
            return Ok("monitor-2");
        }

        [HttpGet]
        [Route("monitor3")]
        public IActionResult monitor3()
        {
            return Ok("monitor-3");
        }

        [HttpGet]
        [Route("monitor4")]
        public IActionResult monitor4()
        {
            return Ok("monitor-4");
        }

        [HttpGet]
        [Route("monitor5")]
        public IActionResult monitor5()
        {
            return Ok("monitor-5");
        }

    }
}
