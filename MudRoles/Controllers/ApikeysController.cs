using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudRoles.Data;
using MudRoles.Data.ApiData;

namespace MudRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApikeysController(ApiKeyDbContext context, ApplicationDbContext applicationContext) : ControllerBase
    {
        private readonly ApiKeyDbContext _context = context;
        private readonly ApplicationDbContext _applicationContext = applicationContext;

        // GET: api/ApiKeys
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ApiKey>>> GetApiKeys()
        {
            var apiKeys = await _context.ApiKeys.ToListAsync();
            return Ok(apiKeys);
        }
    }
}
