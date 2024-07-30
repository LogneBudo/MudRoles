using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MudRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApikeysController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetItem()
        {
            await Task.Yield();
            var response = new { message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit." };

            // Your logic to add an item
            return Ok(response);
        }
    }
}
