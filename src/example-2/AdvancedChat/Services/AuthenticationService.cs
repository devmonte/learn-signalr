using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedChat.Dto;

namespace AdvancedChat.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private static string _secretPassword = "BdgDotNet";

        public Task<bool> Authenticate(UserDto user)
        {
            if (user.Password.Equals(_secretPassword))
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
