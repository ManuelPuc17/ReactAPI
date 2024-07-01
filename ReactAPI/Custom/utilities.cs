using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ReactAPI.Models;

namespace ReactAPI.Custom
{
    public class utilities
    {

        private readonly IConfiguration _config;
        public utilities(IConfiguration config)
        {
            _config = config;
        }

        public string encryptSHA256(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string generateJwt(User model)
        {
            var UserClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Email, model.Email!),

            };

            var securiKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]!));
            var credential = new SigningCredentials(securiKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken(
                claims: UserClaims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credential
             );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
