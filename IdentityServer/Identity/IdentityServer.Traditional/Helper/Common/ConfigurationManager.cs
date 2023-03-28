namespace IdentityServer.Traditional.Helper.Common
{
    public class ConfigurationManager
    {
        public static IConfiguration Configuration { private get; set; }

        public static IConfiguration GetConfiguration()
        {
            return Configuration;
        }
        public static string GetValue(string index)
        {
            return GetValue<string>(index);
        }

        public static T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

        public static string GetConnectionString(string index)
        {
            var res = Configuration.GetConnectionString(index);
            return res;
        }
    }
}
