using Goldrax.Models.Authentication;
using Goldrax.Models.Authentication.MailServiceModels;
using Goldrax.Models.Components;
using Goldrax.Repositories.Authentication.MailServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Goldrax.Repositories.Authentication
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthenticationRepository( UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
            IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Response<object>> SignUpAsync(SignUp signUp)
        {
            var user = new ApplicationUser()
            { 
                FullName = signUp.FullName,
                Email = signUp.Email,
                UserName = signUp.Email,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                
            };

            var result = await _userManager.CreateAsync(user, signUp.Password);

            if(await _roleManager.RoleExistsAsync("Customer"))
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (result.Succeeded)
            {
                return new Response<object>(true, "Check your Email to verify account", new {token,email=user.Email});
            }

            return new Response<object>(false, "SignUp failed", result.Errors);


        }

        //login
        public async Task<object> LoginAsync( SignIn signIn)
        {
            var user = await _userManager.FindByEmailAsync(signIn.Email!);
            if (user == null) return new { status = 404, message = "The User Not Found" };
            if(!await _userManager.CheckPasswordAsync(user, signIn.Password!))
            {
                return new { status = 500, message = "Wrong Password" };
            }
            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, signIn.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var otpMessage = new Message(new string[] { user.Email! }, "OTP Confirmation", token);
                _emailService.SendEmail(otpMessage);
                return new {status= 200, message= $"OTP send to {user.Email}" };
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach(var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            

            //GetToken private generate at the bottom
            var jwtToken = GetToken(authClaims);

            return new
            {
                status = 200,
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            };


        }

        //login-2FA
        public async Task<object> LoginWithOTPAsync(string code, ApplicationUser user)
        {
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }


                //GetToken private generate at the bottom
                var jwtToken = GetToken(authClaims);

                return new
                {
                    status = 200,
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                };
            }
            return new { status = 401, message = "invalid code" };
        }

        //forget password
        public async Task<object> ForgotPasswordAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var message = new Message(new string[] { user.Email! }, "Token To Reset Password",token);
            _emailService.SendEmail(message);
            return new { status = 200, message = $"Reset password token send to {user.Email}" };

        }



        //Jwt Token Generate
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

    }
}
