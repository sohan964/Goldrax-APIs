using Goldrax.Models.Authentication;
using Goldrax.Models.Authentication.MailServiceModels;
using Goldrax.Repositories.Authentication;
using Goldrax.Repositories.Authentication.MailServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Goldrax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationController(IAuthenticationRepository authenticationRepository,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _authenticationRepository = authenticationRepository;
            _roleManager = roleManager;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser([FromBody] SignUp signUp)
        {
            var Result = await _authenticationRepository.SignUpAsync(signUp);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", Result, Request.Scheme);
            var message = new Message(new string[] { signUp.Email! }, "Confirmation email link", confirmationLink!);
            
            _emailService.SendEmail(message);

            return Ok(Result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return NotFound("the user not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignIn signIn)
        {
            var result = await _authenticationRepository.LoginAsync(signIn);
            return Ok(result);
        }

        [HttpPost("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("the user not found");
            var result = await _authenticationRepository.LoginWithOTPAsync(code, user);
            return Ok(result);
        }
    }
}
