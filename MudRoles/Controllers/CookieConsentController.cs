using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace MudRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookieConsentController : ControllerBase
    {
        [HttpPost]
        public IActionResult SetConsent([FromBody] ConsentRequest request)
        {
            var trackingConsentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            if (trackingConsentFeature != null)
            {
                if (request.Consent)
                {
                    trackingConsentFeature.GrantConsent();
                }
                else
                {
                    trackingConsentFeature.WithdrawConsent();
                }
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult GetConsent()
        {
            var trackingConsentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            if (trackingConsentFeature != null)
            {
                return Ok(trackingConsentFeature.CanTrack.ToString().ToLower());
            }

            return Ok("false");
        }
    }
    

    public class ConsentRequest
    {
        public bool Consent { get; set; }
    }
}
