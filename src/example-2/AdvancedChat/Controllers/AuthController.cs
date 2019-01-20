using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedChat.Dto;
using AdvancedChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authorizationService;
        private readonly ITokenProviderService _tokenProviderService;

        public AuthController(IAuthenticationService authorizationService, ITokenProviderService tokenProviderService)
        {
            _authorizationService = authorizationService;
            _tokenProviderService = tokenProviderService;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            var userExist = await _authorizationService.Authenticate(user);

            if (!userExist)
                return NotFound("User doesn't exist!");

            var token = _tokenProviderService.GenerateToken(user);
            return Ok(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(":)");
        }

    }
}
