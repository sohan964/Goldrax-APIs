﻿using Goldrax.Models.Authentication;
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
                TwoFactorEnabled = false,
                EmailConfirmed = true
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
        public async Task<Response<object>> LoginAsync( SignIn signIn)
        {
            var user = await _userManager.FindByEmailAsync(signIn.Email!);
            if (user == null) return new Response<object>(false, "User not found");
            if (!await _userManager.CheckPasswordAsync(user, signIn.Password!))
            {
                return new Response<object>( false, "Wrong Password" );
            }
            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, signIn.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var otpMessage = new Message(new string[] { user.Email! }, "OTP Confirmation", $"<p>{token}</p>");
                _emailService.SendEmail(otpMessage);
                return new Response<object>( true, $"OTP send to {user.Email}" );
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

            return new Response<object>(
            
                true,
                "Login Success",
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                }
            );


        }

        //login-2FA
        public async Task<Response<object>> LoginWithOTPAsync(string code, ApplicationUser user)
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

                return new Response<object>(
                    true,
                    "Login Success",
                    new {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    }
                );
            }
            return new Response<object> ( false, "invalid code" );
        }

        //forget password
        public async Task<Response<object>> ForgotPasswordAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (token == null) new Response<object>(false, $"Fail to forget Password {user.Email}");
            //var message = new Message(new string[] { user.Email! }, "Token To Reset Password",$"<p>{token}</p>");

            var htmlContent = $@"
            <html>
                <body>
                    <h2>Password Reset Token</h2>
                    <p id='token' style='padding:10px; background-color:#f2f2f2; border:1px solid #ccc;'>{token}</p>
                    <button onclick='copyToken()' style='padding:8px 16px; background-color:#4CAF50; color:white; border:none; cursor:pointer;'>Copy Token</button>

                    <script>
                        function copyToken() {{
                            var copyText = document.getElementById('token').innerText;
                            navigator.clipboard.writeText(copyText).then(function() {{
                                alert('Token copied to clipboard!');
                            }}, function(err) {{
                                alert('Failed to copy token');
                            }});
                        }}
                    </script>
                </body>
            </html>";

            var message = new Message(new string[] { user.Email! }, "Token To Reset Password", htmlContent);


            _emailService.SendEmail(message);
            return new Response<object> ( true, $"Reset password token send to {user.Email}" );

        }
        public async Task<Response<object>> UpdateUserAsync(ApplicationUser user)
        {
            var exUser = await _userManager.FindByIdAsync(user.Id);
            if (exUser == null) return new Response<object>(false, "User not found");
            if(user.FullName != string.Empty) exUser.FullName = user.FullName;
            if (user.Address!=string.Empty) exUser.Address = user.Address;
            if (user.PhoneNumber != string.Empty) exUser.PhoneNumber = user.PhoneNumber;
            if (user.City != string.Empty) exUser.City = user.City;
            if (user.Country != string.Empty) exUser.Country = user.Country;
            if (user.PostalCode != string.Empty) exUser.PostalCode = user.PostalCode;

            exUser.LastUpdatedAt = DateTime.UtcNow;

            var res = await _userManager.UpdateAsync(exUser);
            if (res == null) return new Response<object>(false, "Updated Failed");
            return new Response<object>(true, "Updated success");
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
