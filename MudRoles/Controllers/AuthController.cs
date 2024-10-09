using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.WebUtilities;
using MudRoles.Client;
using MudRoles.Client.Models;
using MudRoles.Components.Account;
using MudRoles.Data;
using MudRoles.Infrastructure.Converter;
using Nextended.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static MudRoles.Client.Components.TwoFAuth;
using static MudRoles.Client.Pages.Generics.Signin;

namespace MudRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        private readonly ILogger<AuthController> _logger;
        private readonly IdentityUserAccessor _userAccessor;
        private readonly IAntiforgery _antiforgery;
        // This is a placeholder. Replace it with your actual user service.

        public AuthController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserStore<ApplicationUser> userStore,
        IEmailSender<ApplicationUser> emailSender,
        ILogger<AuthController> logger,
        IdentityUserAccessor userAccessor,
        IAntiforgery antiforgery)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _emailSender = emailSender;
            _logger = logger;
            _userAccessor = userAccessor;
            _antiforgery = antiforgery;
        }

        [HttpGet("user-info")]
        public async Task<ActionResult<UserResponse>> GetUserInfo()
        {
            // Replace this with your actual logic to get user information.
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);

            if (user == null)
            {
                return NotFound();
            }

            var response = new UserResponse
            {
                CanTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? true,
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) is not null,
                Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return Ok(response);
        }

        [HttpPost("forget-browser")]
        [Authorize]
        public async Task<IActionResult> ForgetBrowser()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            await _signInManager.ForgetTwoFactorClientAsync();
            return Ok(new { Message = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code." });
        }

        [HttpPost("enable-2fa")]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Failed to generate 2FA token.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");

            return Ok("Password changed successfully.");
        }

        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return BadRequest("Invalid email change confirmation request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);

            if (!result.Succeeded)
            {
                return BadRequest("Error changing email.");
            }

            // Optionally, you can also update the user's UserName if it is based on the email
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                return BadRequest("Error changing user name.");
            }

            return Ok("Email change confirmed.");
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var profile = new
            {
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Title,
                user.IsDarkTheme,
                user.AvatarId,
                user.AvatarChoice,
                user.UploadedAvatar
            };

            return Ok(profile);
        }

        [HttpPost("update-email")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmailChange", "Auth",
                new { userId, email = model.NewEmail, code },
                HttpContext.Request.Scheme);

            if (callbackUrl == null)
            {
                return BadRequest("Failed to generate confirmation URL.");
            }

            //await _emailSender.SendEmailAsync(model.NewEmail, "Confirm your email change",
            //    $"Please confirm your email change by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok("Confirmation link to change email sent. Please check your email.");
        }

        public class UpdateEmailModel
        {
            [Required]
            [EmailAddress]
            public string NewEmail { get; set; } = "";
        }

        [HttpGet("email")]
        [Authorize]
        public async Task<IActionResult> GetEmail()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var email = await _userManager.GetEmailAsync(user);
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Ok(new { Email = email, IsEmailConfirmed = isEmailConfirmed });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var username = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var avatarChoice = user.AvatarChoice;
            var themeChoice = user.IsDarkTheme;

            return Ok(new { 
                Username = username, 
                PhoneNumber = phoneNumber, 
                FirstName = firstName, 
                LastName = lastName, 
                ThemeChoice = themeChoice,
                AvatarChoice = avatarChoice });
        }

        // POST: api/auth/signin
        [HttpPost("SignIn")]
        public async Task<ActionResult<SignInModel>> SignIn(SignInModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                
                return Ok(new SignInResponse { Message = "Sign-in successful" });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status423Locked, "User account locked out");
            }

            return Unauthorized("Invalid login attempt");
        }

        // POST: api/Auth/SignOut
        [HttpPost("SignOut")]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            await _signInManager.SignOutAsync();
            return Ok("Sign-out successful");
        }

        [HttpPost("sendconflink")]
        public async Task<IActionResult> SendConfirmationLink(string email, string callbackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            
            if (callbackUrl == null)
            {
                return BadRequest("Failed to generate confirmation URL.");
            }
            await _emailSender.SendConfirmationLinkAsync(user, email, callbackUrl);
            return Ok("Confirmation link sent. Please check your email.");
        }

        // POST: api/Auth/SignUp
        [HttpPost("SignUp")]
        public async Task<ActionResult<InputModel>> Register(InputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Title = model.Title,
                IsDarkTheme = model.IsDarkTheme,
                AvatarId = model.AvatarId,
                AvatarChoice = model.AvatarChoice,
                UploadedAvatar = model.AvatarPhoto
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("User created a new account with password.");

            // Assign the "User" role to the newly registered user
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            return Ok(new RegisterResponse
            {
                UserId = userId,
                Code = code,
                RequireConfirmedAccount = _userManager.Options.SignIn.RequireConfirmedAccount
            });
        }

        [HttpPost("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Title = model.Title;
            user.IsDarkTheme = model.IsDarkTheme;
            user.AvatarId = model.AvatarId;
            user.AvatarChoice = model.AvatarChoice;
            user.UploadedAvatar = model.UploadedAvatar;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Error updating profile: {errors}");
            }

            return Ok("Profile updated successfully.");
        }

        [HttpGet("GetToken")]
        public IActionResult GetToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { token = tokens.RequestToken });
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest("Error deleting account.");
            }

            return Ok("Account deleted successfully.");
        }

        [HttpGet("status-message")]
        public IActionResult GetStatusMessage()
        {
            var message = Request.Cookies[IdentityRedirectManager.StatusCookieName];
            if (message != null)
            {
                Response.Cookies.Delete(IdentityRedirectManager.StatusCookieName);
            }
            return Ok(message);
        }

        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountModel model)
        {
            if (model.UserId == null || model.Code == null)
            {
                return BadRequest("Invalid activation request.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Ok("Account activated successfully.");
            }

            return BadRequest("Error activating account.");
        }
    }
    public class ActivateAccountModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }
    public class SignInResponse
    {
        public string Message { get; set; } = string.Empty;
    }
    public class RegisterResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool RequireConfirmedAccount { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
    public class UpdateProfileModel
    {
        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        public string? Title { get; set; }

        public bool IsDarkTheme { get; set; }

        public string? AvatarId { get; set; }

        public string? AvatarChoice { get; set; }

        public byte[]? UploadedAvatar { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        public string? Title { get; set; }

        public bool IsDarkTheme { get; set; }

        public string? AvatarId { get; set; }
    }
}

