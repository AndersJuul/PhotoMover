using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AJF.PhotoMover.Service
{
    public class SettingsWrapper
    {
        private readonly XmlDocument _xmlSettings;
        private bool _removeEmptySourceDir;

        public SettingsWrapper(XmlDocument xmlSettings)
        {
            _xmlSettings = xmlSettings;
        }

        public IEnumerable<string> SearchPatterns
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/SearchPatterns/SearchPattern").Cast<XmlNode>();
                var enumerable = from xn in xmlNodeList select xn.Attributes["value"].Value;
                var patterns = enumerable.ToList();
                return patterns;
            }
        }

        public string TestFile
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/TestFile");
                var xmlNode = xmlNodeList[0];
                var xmlAttribute = xmlNode.Attributes["value"];
                var value = xmlAttribute.Value;  

                return value;
            }
        }

        public string Destination
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/Destination");
                var xmlNode = xmlNodeList[0];
                var xmlAttribute = xmlNode.Attributes["value"];
                var value = xmlAttribute.Value;

                return value;
            }
        }

        public string NewExtension
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/NewExtension");
                if (xmlNodeList.Count==0)
                    return null;

                var xmlNode = xmlNodeList[0];
                var xmlAttribute = xmlNode.Attributes["value"];
                var value = xmlAttribute.Value;

                return value;
            }
        }

        public int MinAgeHours
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/MinAgeHours");
                var xmlNode = xmlNodeList[0];
                var xmlAttribute = xmlNode.Attributes["value"];
                var value = xmlAttribute.Value;

                return Convert.ToInt32( value);
            }
        }

        public bool RemoveEmptySourceDir
        {
            get
            {
                var xmlNodeList = _xmlSettings.SelectNodes("/Settings/RemoveEmptySourceDir");
                if (xmlNodeList.Count == 0)
                    return false;
                var xmlNode = xmlNodeList[0];
                var xmlAttribute = xmlNode.Attributes["value"];
                var value = xmlAttribute.Value;

                return Convert.ToBoolean(value);
            }
        }
    }
}