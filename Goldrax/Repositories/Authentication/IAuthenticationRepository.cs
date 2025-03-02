using Goldrax.Models.Authentication;

namespace Goldrax.Repositories.Authentication
{
    public interface IAuthenticationRepository
    {
        Task<object> SignUpAsync(SignUp signUp);
        Task<object> LoginAsync(SignIn signIn);
        Task<object> LoginWithOTPAsync(string code, ApplicationUser user);
    }
}
