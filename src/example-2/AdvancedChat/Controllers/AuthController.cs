﻿using AdvancedChat.Dto;
using AdvancedChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdvancedChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenProviderService _tokenProviderService;

        public AuthController(ITokenProviderService tokenProviderService)
        {
            _tokenProviderService = tokenProviderService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDto user)
        {

            if (!string.Equals(user.Password, "dotnet"))
                return StatusCode(403, "Invalid password!");

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
