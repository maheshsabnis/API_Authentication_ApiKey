using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Authentication_ApiKey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello");
        }
    }
}
