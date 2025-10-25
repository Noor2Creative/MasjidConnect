namespace MasjidConnect.Infranstructure.DbHelpers
{
    internal class ProcedureManager
    {
        public const string InsertOrUpdateUser = "SP_InsertOrUpdateUser_Set";
        public const string GetUserByUserName = "SP_GetUserByUsername";
        public const string GetUserByUserId = "SP_GetUserByUserid";
        public const string InsertOrUpdateMasjid = "SP_InsertOrUpdateMasjid_set";
        public const string InsertOrUpdateNamazTime = "sp_InsertOrUpdateNamazTime";
        public const string SpGetAllMasjid = "SP_GetAllMasjid";
        public const string GetMasjid = "SP_GetMasjidById";
    }
}
