namespace Blog.Areas.Identity.Data
{
    public class Roles
    {
        public readonly static string Admin = "Admin";
        public readonly static string Moderator = "Moderator";
        public readonly static string User = "User";

        public static string[] AllRoles = new string[] { Admin, User, Moderator };
    }
}
