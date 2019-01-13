using AdvancedChat.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedChat.Services
{
    public interface ITokenProviderService
    {
        string GenerateToken(UserDto user);
    }
}
