using Goldrax.Models.Authentication;
using Goldrax.Models.Authentication.MailServiceModels;
using Goldrax.Models.Components;
using Goldrax.Repositories.Authentication;
using Goldrax.Repositories.Authentication.MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> CreateUser([FromBody] SignUp? signUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>(false, "Validation Failed", ModelState));
            }
            try
            {
                

                var Result = await _authenticationRepository.SignUpAsync(signUp);

                if(Result.Succeeded == false)
                {
                    return BadRequest(Result);
                }

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", Result.Data, Request.Scheme);
                var message = new Message(new string[] { signUp.Email! }, "Confirmation email link",
                    "<h1>Welcome!</h1><p>Please confirm your email by clicking <a href='" + confirmationLink + "'>here</a>.</p>");

                _emailService.SendEmail(message);
                
                return Ok(new Response<object>(Result.Succeeded, Result.Message!));
            }
            catch (Exception ex)
            {

                return Unauthorized(new Response<object>(false, ex.Message, ex.Data));
            }
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
            if(result.Succeeded)
            {
               return  Ok(result);
            }
            return Unauthorized(result);
        }

        [HttpPost("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new Response<object>(false,"the user not found"));
            var result = await _authenticationRepository.LoginWithOTPAsync(code, user);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }

        //get current user
        [HttpGet("user")]
        public async Task<IActionResult> CurrentUser()
        {
            var email = HttpContext.User?.Claims.First().Value;
            if(email == null) return Unauthorized("Not a Valid Token");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("user not found");
            var role = await _userManager.GetRolesAsync(user);
            return Ok(

                new Response<object>(true, "The current user",
                new { 
                user.FullName,
                user.Email,
                user.Id,
                user.EmailConfirmed,
                user.TwoFactorEnabled,
                role,
                user.Address,
                user.City,
                user.Country,
                user.PostalCode,
                user.PhoneNumber,
            
            }));
        }

        [HttpPost("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("user not found");
            var result = await _authenticationRepository.ForgotPasswordAsync(user);
            if(result.Succeeded) { return Ok(result); }
            return BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email!);
            if (user == null) return NotFound( new Response<object>( false,"user not found"));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token!, resetPassword.Password!);
            if(!resetPassResult.Succeeded)
            {
                foreach(var error in resetPassResult.Errors)
                {
                    //modelState builtin here
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(new Response<object>(false, "Fatal Error", ModelState));
            }
            return Ok(new Response<object>(resetPassResult.Succeeded, "Password Reset successed", resetPassResult));

        }

        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] ApplicationUser user)
        {
            var result = await _authenticationRepository.UpdateUserAsync(user);
            if (!result.Succeeded) { return BadRequest(result); }
            return Ok(result);
        }

    }
}
