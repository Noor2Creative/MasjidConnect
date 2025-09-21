using System.ComponentModel;
using System.Reflection;

namespace MasjidConnect.Application
{
    public static class Enums
    {
        public enum StatusType
        {
            [Description("Success")]
            Success,

            [Description("Fail")]
            Fail,

            [Description("Exist")]
            Exist
        }
        public enum ErrorType
        {
            [Description("Error001:No Record Found")]
            NoRecord,
            [Description("Error002:Internal Server Code")]
            InternalServerError,
            [Description("Error003:Invalid Username or Password")]
            InvalidUser,
            [Description("Error004:You are unauthorized.")]
            Unauthorized,
            [Description("Error005:Token is missing")]
            TokenMissing,
            [Description("Error006:Token is expired")]
            TokenExpired,
            [Description("Error007:Invalid Token")]
            InvalidToken,
            [Description("Error008:Invalid File Format")]
            InvalidFileFormat,
            [Description("200:Valid Token")]
            ValidToken
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
