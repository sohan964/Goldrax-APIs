using Goldrax.Models.Authentication;
using Goldrax.Repositories.Authentication;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationController(IAuthenticationRepository authenticationRepository,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _authenticationRepository = authenticationRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser([FromBody] SignUp signUp)
        {
            var Result = await _authenticationRepository.SignUpAsync(signUp);

            return Ok(Result);
        }
    }
}
