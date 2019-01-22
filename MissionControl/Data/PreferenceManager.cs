using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MissionControl.Data
{

    // Singleton pattern
    // http://csharpindepth.com/articles/general/singleton.aspx

    public sealed class PreferenceManager
    {

        public const string STD_PORTNAME = "PortName";
        public const string STD_LOGFOLDER = "LogFolder";

        private string _filepath;
        private Dictionary<string, string> _preferences;

        public static PreferenceManager Preferences { get; } = new PreferenceManager();

        public string this [string key]
        {
            get {
                return _preferences.ContainsKey(key) ? _preferences[key] : null;
            }
            set { _preferences[key] = (string) value; }
        }

        static PreferenceManager()
        {
        }

        public PreferenceManager()
        {
            _filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"preferences.txt");
            _preferences = new Dictionary<string, string>();

            if (File.Exists(_filepath))
            {
                string pattern = @"(.*)\s?:\s?(.*)";
                RegexOptions options = RegexOptions.Multiline;

                foreach (string line in File.ReadAllLines(_filepath))
                {
                    string[] values = Regex.Split(line, pattern, options);

                    // Split creates empty entries before and after
                    if (values.Length != 4)
                    {
                        Console.WriteLine("\"{0}\" is not split into 4", line);
                        continue;
                    }

                    _preferences.Add(values[1], values[2]);
                }
            }
        }

        public void Remove(string key)
        {
            if (_preferences.ContainsKey(key))
            {
                _preferences.Remove(key);
            }
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(_filepath, false))
            {
                foreach (KeyValuePair<string, string> kv in _preferences)
                {
                    sw.WriteLine(string.Format("{0}:{1}", kv.Key, kv.Value));
                }
            }
        }
    }
}
