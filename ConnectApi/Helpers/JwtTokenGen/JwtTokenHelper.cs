using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using ConnectApi.Helpers.JsonResponse;
using ConnectApi.Models.Users;
using ConnectApi.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ConnectApi.Helpers.JwtTokenGen
{
    public class JwtTokenHelper
    {
        public string GenerateToken(User userInDb)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.MaximumTokenSizeInBytes = AppConstants.TokenHandlerSize;
            var key = Encoding.ASCII.GetBytes(AppConstants.AppSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInDb.Id.ToString()),
                    new Claim(ClaimTypes.Name, userInDb.Email)
                }),

                Expires = DateTime.Now.AddMinutes(AppConstants.TokenExpireTimeDuration), // Token Never Expires Requirement
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // Create JWT Token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
