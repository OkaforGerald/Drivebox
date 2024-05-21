using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using SharedAPI.DataTransfer;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private User? User;

        public AuthService(UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto newUser)
        {
            var user = mapper.Map<User>(newUser);  

            var result = await userManager.CreateAsync(user, newUser.Password);

            return result;
        }

        public async Task<bool> AuthenticateUserAsync(AuthenticateUserDto User)
        {
            this.User = await userManager.FindByEmailAsync(User.Email);

            return this.User is not null && await userManager.CheckPasswordAsync(this.User, User.Password);
        }

        public async Task<TokenDto> GenerateAccessTokens(bool refrehExp)
        {
            var claims = GetClaims();
            var creds = GetSigningCreds();
            var jwtSettings = configuration.GetSection("JwtSettings");
            User.RefreshToken = GenerateRefreshToken();
           
            if(refrehExp) { User.RefreshTokenExpiry = DateTime.Now.AddDays(7); }

            var jwtSecurityToken = new JwtSecurityToken(issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            await userManager.UpdateAsync(User);

            return new TokenDto { AccessToken = accessToken, RefreshToken = User.RefreshToken };
        }

        private List<Claim> GetClaims()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, User.UserName),
                new Claim(ClaimTypes.Email, User.Email)
            };

            return claims;
        }

        private SigningCredentials GetSigningCreds()
        {
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings")["SigningKey"])), SecurityAlgorithms.HmacSha256);
            return creds;
        }

        private string GenerateRefreshToken()
        {
            var arr = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(arr);
            }

            return Convert.ToBase64String(arr);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,

                ValidAudience = configuration.GetSection("JwtSettings")["ValidAudience"],
                ValidIssuer = configuration.GetSection("JwtSettings")["ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings")["SigningKey"]))
            };

            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            var token = securityToken as JwtSecurityToken;

            if(principal is null || !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("Couldn't validate access token!");
            }

            return principal;
        }

        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
        {
            var principal = GetClaimsPrincipal(tokenDto.AccessToken);

            var user = await userManager.FindByNameAsync(principal?.Identity?.Name);

            if (user == null || !user.RefreshToken.Equals(tokenDto.RefreshToken) || user.RefreshTokenExpiry < DateTime.Now)
            {
                throw new Exception("Refreh token action failed!");
            }

            User = user;
            return await GenerateAccessTokens(refrehExp: false);
        }
    }
}
