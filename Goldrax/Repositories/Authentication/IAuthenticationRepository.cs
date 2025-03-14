using Goldrax.Models.Authentication;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.Authentication
{
    public interface IAuthenticationRepository
    {
        Task<Response<object>> SignUpAsync(SignUp signUp);
        Task<object> LoginAsync(SignIn signIn);
        Task<object> LoginWithOTPAsync(string code, ApplicationUser user);
        Task<object> ForgotPasswordAsync(ApplicationUser user);
    }
}
