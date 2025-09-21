
using MasjidConnect.Model.Request.User;
using MasjidConnect.Model.Response.User;
using MasjidConnect.Model.Response;

namespace MasjidConnect.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ResponseHeader> RegisterUserAsync(RegisterRequest model);
        Task<UserResponse?> ValidateUserAsync(LoginRequest model);
        Task<UserResponse?> GetUserByIdAsync(int id);
    }
}
