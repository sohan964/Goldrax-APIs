﻿using Goldrax.Models.Authentication;

namespace Goldrax.Repositories.Authentication
{
    public interface IAuthenticationRepository
    {
        Task<object> SignUpAsync(SignUp signUp);
    }
}
