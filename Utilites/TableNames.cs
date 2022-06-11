namespace LogBackend.Utilities;
public static class UserConstants{
     public static string Id = nameof(Id);
    public static string Name = nameof(Name);
    public static string Email = nameof(Email);
    public static string IsSuperuser = nameof(IsSuperuser);
}


    public enum TableNames
   {
       user,
       log,
       application_api_key,
       log_seen,
       tag,
       tag_type,
       user_login,
       user_tag,
       log_tag
   }