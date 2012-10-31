namespace Deployd.Agent.Bootstrap
{
    public class DeploydVersion
    {
        public string Id { get { return "DeploydVersion/1"; } }
        public int MigrationsVersion { get; set; }
        public bool ConfigurationRequired { get; set; }
    }
}