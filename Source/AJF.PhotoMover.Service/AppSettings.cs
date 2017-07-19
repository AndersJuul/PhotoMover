using System;
using System.Configuration;
using Ajf.Nuget.Logging;

namespace AJF.PhotoMover.Service
{
    public class AppSettings : ServiceSettingsFromConfigFile, IAppSettings
    {
        public AppSettings()
        {
            PerformMove = ConfigurationManager.AppSettings["PerformMove"] == "1";
            TickSleep =Convert.ToInt32( ConfigurationManager.AppSettings["TickSleep"]);
        }

        public bool PerformMove { get; set; }

        public int TickSleep { get; set; }
    }
}