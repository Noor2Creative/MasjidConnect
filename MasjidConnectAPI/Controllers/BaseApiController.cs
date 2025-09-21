using MasjidConnect.Application;
using MasjidConnect.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace MasjidConnectAPI.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private readonly string _jwtSecret;

        public BaseApiController(IConfiguration config)
        {
            _jwtSecret = config["JwtSettings:Key"];
        }
        //validate token and return claims
        protected (bool, ResponseHeader, Dictionary<string, string>) ValidateTokeAndGetClaims(string bearerToken, params string[] claimsTobeExtracted)
        {

            ResponseHeader responseHeader = new ResponseHeader();
            Dictionary<string, string> claims = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(bearerToken))
            {
                responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                responseHeader.Code = Enums.ErrorType.TokenMissing.ToString();
                responseHeader.Message = Enums.ErrorType.TokenMissing.GetDescription(); ;
                return (false, responseHeader, claims);
            }
            // Strip "Bearer " prefix if present
            var token = bearerToken?.StartsWith("Bearer ") == true ? bearerToken.Substring(7) : bearerToken;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                    //ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                if (claimsTobeExtracted?.Length > 0)
                {
                    foreach (var claimName in claimsTobeExtracted)
                    {
                        var value = jwtToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                        if (value != null)
                        {
                            claims[claimName] = value;
                        }
                    }
                }
                else
                {
                    claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
                }

                responseHeader.Status = Enums.StatusType.Success.GetDescription();
                responseHeader.Code = Enums.ErrorType.ValidToken.GetDescription().Split(':')[0];
                responseHeader.Message = Enums.ErrorType.ValidToken.GetDescription().Split(':')[1];

                return (true, responseHeader, claims);
            }
            catch (SecurityTokenExpiredException)
            {
                responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                responseHeader.Code = Enums.ErrorType.TokenExpired.ToString();
                responseHeader.Message = Enums.ErrorType.TokenExpired.GetDescription().Split(':')[1];
                return (false, responseHeader, claims);
            }
            catch (Exception)
            {
                responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                responseHeader.Code = Enums.ErrorType.InvalidToken.ToString();
                responseHeader.Message = Enums.ErrorType.InvalidToken.GetDescription().Split(':')[1];
                return (false, responseHeader, claims);
            }
        }
    }
}
