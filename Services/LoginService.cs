using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FashionStoreAPI.Services
{
    public class LoginService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenResponse?> AuthenticateAsync(LoginRequest request)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (myUser == null)
                return null; // User not found
                             
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, myUser.Password);

            if (!isValidPassword)
                return null; // Invalid password

            var signingKey = Convert.FromBase64String(_configuration["JWT:SigningSecret"] ?? throw new InvalidOperationException("JWT SigningSecret är inte konfigurerad."));

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, myUser.Id.ToString()),
                new (ClaimTypes.Role, myUser.IsAdmin ? "Admin" : "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims)
            };

            var jwtHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = jwtHandler.CreateJwtSecurityToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            {
                Token = jwtHandler.WriteToken(jwtSecurityToken),
                FirstName = myUser.FirstName,
                LastName = myUser.LastName
            };
            
            return tokenResponse;
        }
    }
}
