using System.Configuration;
using Ajf.Nuget.Logging;

namespace AJF.PhotoMover.Service
{
    public class AppSettings : ServiceSettingsFromConfigFile, IAppSettings
    {
        public AppSettings()
        {
            PerformMove = ConfigurationManager.AppSettings["PerformMove"] == "1";
        }

        public bool PerformMove { get; set; }
    }
}