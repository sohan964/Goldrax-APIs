using Goldrax.Models.Authentication;
using Goldrax.Models.Components;

namespace Goldrax.Repositories.Authentication
{
    public interface IAuthenticationRepository
    {
        Task<Response<object>> SignUpAsync(SignUp signUp);
        Task<Response<object>> LoginAsync(SignIn signIn);
        Task<Response<object>> LoginWithOTPAsync(string code, ApplicationUser user);
        Task<Response<object>> ForgotPasswordAsync(ApplicationUser user);
    }
}
