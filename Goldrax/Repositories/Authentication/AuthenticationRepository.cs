using Goldrax.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Goldrax.Repositories.Authentication
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationRepository( UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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

            var result = await _userManager.CreateAsync(user, signUp?.Password);

            if(await _roleManager.RoleExistsAsync("Customer"))
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return result;


        }
    }
}
