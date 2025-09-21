

namespace MasjidConnect.Model.Response.User
{
    public class UserResponse
    {
        public ResponseHeader Header { get; set; } = new ResponseHeader();
        public UserDto? User { get; set; }
    }
    public class UserDto
    {
        public int Id { get; set; }            // Database UserId
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; } = 0;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
