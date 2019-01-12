using AdvancedChat.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedChat.Services
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(UserDto user);
    }
}
