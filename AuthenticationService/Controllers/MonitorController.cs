﻿using Microsoft.AspNetCore.Http;
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

    }
}
