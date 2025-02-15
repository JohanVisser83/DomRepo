using RepoDb.Attributes;

namespace Circular.Core.Entity
{
    [Map("User")]
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
    public class OpenIDIdentityResponse
    {
        public string value { get; set; }
    }
    public class LogOut
    {
        public string token { get; set; }
    }

    public class OpenIDLoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }

        public Int64 expires_in { get; set; }

        public string scope { get; set; }

        public string id_token { get; set; }
        public string refresh_token { get; set; }

        public string User_Code { get; set; }
    }
}
