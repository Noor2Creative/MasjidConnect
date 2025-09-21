using MasjidConnect.Application.Interfaces;
using MasjidConnect.Infranstructure.DbHelpers;
using MasjidConnect.Model.Request.User;
using MasjidConnect.Model.Response;
using MasjidConnect.Model.Response.User;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MasjidConnect.Infranstructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbExecuter _dbExecuter;
        private readonly IConfiguration _configuration;
        public UserRepository(IDbExecuter dbExecuter, IConfiguration configuration)
        {
            _dbExecuter = dbExecuter;
            _configuration = configuration;
        }


        public async Task<ResponseHeader> RegisterUserAsync(RegisterRequest model)
        {
            try
            {
                ResponseHeader responseHeader = new ResponseHeader();
                //validate model

                //generate slat
                var salt = CreateSalt();

                //create hash password
                string PasswordHash = CreateHashPassword(model.Password, salt);

                var parameters = new[]
                {
                    new SqlParameter("@Username", model.Username),
                    new SqlParameter("@PasswordHash", PasswordHash),
                    new SqlParameter("@salt", salt),
                    new SqlParameter("@fullname", model.FullName ?? (object)DBNull.Value),
                    new SqlParameter("@email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@Role", model.RoleId),// assumes 1=Admin,2=User,3=Guest
                };

                DataTable result = await _dbExecuter.ExecuteProcedureDtAsync(ProcedureManager.InsertOrUpdateUser, parameters);
                if (result != null && result.Rows.Count > 0 && result.Rows[0]["flag"].ToString().ToLower() == "ok")
                {
                    responseHeader.Status = "Success";
                    responseHeader.Message = result.Rows[0]["msg"].ToString();
                }
                else
                {
                    responseHeader.Status = "Fail";
                    responseHeader.Message = result.Rows[0]["msg"].ToString();
                }
                return responseHeader;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserResponse?> ValidateUserAsync(LoginRequest model)
        {
            UserResponse response = new UserResponse
            {
                Header = new ResponseHeader { Status = "Fail", Message = "Invalid Username or Password" },
                User = null
            };
            try
            {
                //1.validate model                
                string Username, Password; int RoleId;

                Username = model.Username;
                Password = model.Password;
                RoleId = model.RoleId;

                var parameters = new[]
                {
                    new SqlParameter("@Username", Username),
                    new SqlParameter("@RoleId", RoleId),// assumes 1=Admin,2=User,3=Guest
                };

                using (SqlDataReader result = await _dbExecuter.ExecuteProcedureDrAsync(ProcedureManager.GetUserByUserName, parameters))
                {
                    if (await result.ReadAsync())
                    {
                        //await result.ReadAsync();
                        //get and compare and password
                        string dbPasswordHash = result["PasswordHash"].ToString();
                        string dbPasswordSalt = result["PasswordSalt"].ToString();

                        if (!IsPasswordValid(Password, dbPasswordHash, dbPasswordSalt))
                            return response;


                        // If password is valid, populate the user details.
                        response.User = new UserDto
                        {
                            Id = Convert.ToInt32(result["Id"]),
                            Username = result["Username"].ToString(),
                            FullName = result["FullName"].ToString(),
                            RoleId = Convert.ToInt32(result["RoleId"]),
                            Role = result["RoleName"].ToString()
                        };

                        //generate Token and set into user dto
                        string token = GenerateJwtToken(response.User);
                        response.User.Token = token;
                        response.Header.Status = "Success";
                        response.Header.Message = "User logged in successfully.";
                    }
                    else
                    {
                        return response;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                // Return a generic error response to the client.
                response.Header.Status = "Fail";
                response.Header.Message = $"Error Message: {ex.Message} \n Error Info: {ex.StackTrace}";
                return response;
            }
        }
        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            UserResponse response = new UserResponse
            {
                Header = new ResponseHeader { Status = "Fail", Message = "Invalid User/" },
                User = null
            };
            try
            {
                //1.validate model                
               

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                using (SqlDataReader result = await _dbExecuter.ExecuteProcedureDrAsync(ProcedureManager.GetUserByUserId, parameters))
                {
                    if (await result.ReadAsync())
                    {
                        // If password is valid, populate the user details.
                        response.User = new UserDto
                        {
                            Id = Convert.ToInt32(result["Id"]),
                            Username = result["Username"].ToString(),
                            FullName = result["FullName"].ToString(),
                            RoleId = Convert.ToInt32(result["RoleId"]),
                            Role = result["RoleName"].ToString()
                        };

                        response.Header.Status = "Success";
                        response.Header.Message = "User details retrived successfully.";
                    }
                    else
                    {
                        return response;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                // Return a generic error response to the client.
                response.Header.Status = "Fail";
                response.Header.Message = $"Error Message: {ex.Message} \n Error Info: {ex.StackTrace}";
                return response;
            }
        }

        private static string CreateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt); ;
        }

        private static string CreateHashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var hmac = new HMACSHA512(saltBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        private bool IsPasswordValid(string enteredPassword, string storedHash, string storedSalt)
        {
            // Check for null or empty values
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
            {
                return false;
            }

            // Hash the entered password with the stored salt
            string enteredPasswordHash = CreateHashPassword(enteredPassword, storedSalt);

            // Compare the new hash with the stored hash
            return enteredPasswordHash == storedHash;
        }

        private string GenerateJwtToken(UserDto user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim("RoleId", user.RoleId.ToString()),
            new Claim("Role", user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
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
    }
}
