using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendcafe.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Selamat datang di CafeMobile API" });
        }
    }
}