using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ebookhub.Data;
using ebookhub.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ebookhub.Infrastructure
{
    public class AuthenticationTools
    {
        private readonly TokenOptions _tokenOptions;

        public AuthenticationTools(IOptions<TokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
        }

        public string GenerateToken(User user, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.Name, "TokenAuth"),
                new[] { new Claim("ID", user.Id) }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenOptions.Issuer,
                Audience = _tokenOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Key)),
                    SecurityAlgorithms.HmacSha256),
                Subject = identity,
                Expires = expires
            });
            return handler.WriteToken(securityToken);
        }

        public void HashPassword(User user)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            user.Salt = Convert.ToBase64String(salt);

            user.Password = GenerateHashedPassword(user.Password, Convert.FromBase64String(user.Salt));
        }

        public string GenerateHashedPassword(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public bool CheckPassword(User user, string password)
        {
            string hashed = GenerateHashedPassword(password, Convert.FromBase64String(user.Salt));
            return hashed == user.Password;
        }
    }
}
