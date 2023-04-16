using Microsoft.Extensions.Configuration;

namespace MatrixVision.Connector.Core
{
    public static class ConfigurationAccessor
    {
        public static IConfiguration? Configuration { get; set; }

        public static bool VirtualDeviceEnabled
        {
            get
            {
                var success = bool.TryParse(Configuration?["VirtualDeviceSettings:Enabled"], out bool enabled);

                if(success)
                {
                    return enabled;
                }

                return success;
            }
        }

        public static string? FolderPath => Configuration?["VirtualDeviceSettings:ImagesFolderPath"];
    }
}
