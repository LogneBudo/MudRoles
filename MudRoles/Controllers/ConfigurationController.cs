using Microsoft.AspNetCore.Mvc;

namespace MudRoles.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("apikey")]
        public IActionResult GetApiKey()
        {
            var apiKey = _configuration["ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return NotFound("API Key not found");
            }
            return Ok(apiKey);
        }
    }
}
