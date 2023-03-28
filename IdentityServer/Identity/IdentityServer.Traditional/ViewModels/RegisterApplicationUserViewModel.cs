namespace IdentityServer.Traditional.ViewModels
{
    public class RegisterApplicationUserViewModel
    {
        public string SecurityStamp { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
