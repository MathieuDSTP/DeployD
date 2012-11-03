namespace Deployd.Core.AgentConfiguration
{
    public class XmppSettings : IXMPPSettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Recipients { get; set; }
    }
}