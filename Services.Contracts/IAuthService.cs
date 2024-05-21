using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SharedAPI.DataTransfer;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto newUser);

        Task<bool> AuthenticateUserAsync(AuthenticateUserDto User);

        Task<TokenDto> GenerateAccessTokens(bool refrehExp);

        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    }
}
