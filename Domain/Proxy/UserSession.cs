namespace TMPP_CRM.Domain.Proxy
{
    public class UserSession
    {
        public string Username { get; set; }
        public string Role { get; set; }

        public UserSession(string username, string role)
        {
            Username = username;
            Role = role;
        }
    }
}
