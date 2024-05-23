using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using SharedAPI;
using SharedAPI.DataTransfer;

namespace Dropbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public AuthController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new List<string>
                    {
                        "Registration object can not be null!"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await serviceManager.AuthService.RegisterUserAsync(registerUserDto);
            if(!result.Succeeded)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = result.Errors.Select(x => x.Description).ToList()
                });
            }

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Registration successful!"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserDto details)
        {
            if (details == null)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new List<string>
                    {
                        "Login object can not be null!"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isLoginSuccessful = await serviceManager.AuthService.AuthenticateUserAsync(details);

            if (!isLoginSuccessful)
            {
                return Unauthorized(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Errors = new List<string> { "Invalid Username/Password" }
                });
            }

            var accessTokens = await serviceManager.AuthService.GenerateAccessTokens(refrehExp: true);

            return Ok(
                new ResponseDto<TokenDto>{
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = accessTokens
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokens([FromBody] TokenDto token)
        {
            if (token is null)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new List<string>
                    {
                        "Login object can not be null!"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accessTokens = await serviceManager.AuthService.RefreshTokenAsync(token);

            return Ok(new ResponseDto<TokenDto>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = accessTokens
            });
        }
    }
}
