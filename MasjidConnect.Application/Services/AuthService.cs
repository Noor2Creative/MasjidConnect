using MasjidConnect.Application.Interfaces;
using MasjidConnect.Model.Request.User;
using MasjidConnect.Model.Response.User;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
namespace MasjidConnect.Application.Services
{
    public class AuthService
    {
        /*
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Get user from DB
            var (status, userDto) = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (status == LoginResultStatus.UserNotFound)
                return new LoginResponse { Success = false, Message = "User not found" };

            // Fetch password info again (raw SQL or extend repo to include hash+salt)
            // Example: pretend we retrieved them
            string storedHash = "db_password_hash_here";
            string storedSalt = "db_salt_here";

            if (!VerifyPassword(request.Password, storedSalt, storedHash))
                return new LoginResponse { Success = false, Message = "Invalid password" };

            // Generate JWT
            string token = GenerateJwtToken(userDto);

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userDto
            };
        }

        private bool VerifyPassword(string password, string salt, string storedHash)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHashStr = Convert.ToBase64String(computedHash);
            return computedHashStr == storedHash;
        }

        private string GenerateJwtToken(UserDto user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        */
    }
}