using Goldrax.Models.Authentication;
using Goldrax.Repositories.Authentication.MailServices;
using Microsoft.AspNetCore.Identity;

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

        public async Task<object> SignUpAsync(SignUp signUp)
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
                return new
                {
                    token,
                    email = user.Email

                };
            }

            return result;


        }
    }
}
