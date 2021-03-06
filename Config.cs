using System.IO;
using Newtonsoft.Json;

namespace Ecommerce_API
{
    public class Config
    {
        private const string FILE_NAME = "api.config.json";
        public string DBConnectionString { get; set; }

        private static Config _config = null;

        private Config()
        {

        }

        public static Config GetInstance()
        {
            if (_config == null)
            {
                var json = File.ReadAllText(FILE_NAME);
                _config = JsonConvert.DeserializeObject<Config>(json);
            }

            return _config;
        }
    }
}