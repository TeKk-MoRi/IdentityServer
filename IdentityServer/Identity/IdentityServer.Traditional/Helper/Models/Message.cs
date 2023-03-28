using IdentityServer.Traditional.Helper.Enums;

namespace IdentityServer.Traditional.Helper.Models
{
    public class Message
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public MessageType Type { get; set; }
    }
}
