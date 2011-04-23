namespace NBlog.Web.Application.Service.Entity
{
    public class User
    {
        public string Username { get; set; }
        public string FriendlyName { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsAdmin { get; set; }
    }
}