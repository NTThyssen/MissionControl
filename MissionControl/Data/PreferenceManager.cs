using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MissionControl.Data
{

    // Singleton pattern
    // http://csharpindepth.com/articles/general/singleton.aspx

    public sealed class PreferenceManager
    {
  
        private string _filepath;
        private Dictionary<string, string> _preferences;

        public static PreferenceManager Preferences { get; } = new PreferenceManager();

        public string this [string key]
        {
            get {
                return _preferences.ContainsKey(key) ? _preferences[key] : null;
            }
            set { _preferences[key] = value; }
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
                    sw.WriteLine("{0}:{1}", kv.Key, kv.Value);
                }
            }
        }

        public static int GetIfExists(string key, int defaultValue)
        {
            string sval = Preferences[key];
            return (sval != null && int.TryParse(sval, out int ival)) ? ival : defaultValue;
        }

        public static float GetIfExists(string key, float defaultValue)
        {
            string sval = Preferences[key];
            return (sval != null && float.TryParse(sval, NumberStyles.Any, CultureInfo.InvariantCulture, out float fval)) ? fval : defaultValue;
        }

        public static string GetIfExists(string key, string defaultValue)
        {
            return Preferences[key] ?? defaultValue;
        }

        public static bool GetIfExists(string key, bool defaultValue)
        {
            string sval = Preferences[key];
            return (sval != null && bool.TryParse(sval, out bool bval)) ? bval : defaultValue;
        }

        public static void Set(string key, string value)
        {
            Preferences[key] = value;
        }

        public static void Set(string key, int value)
        {
            Preferences[key] = Convert.ToString(value);
        }

        public static void Set(string key, float value)
        {
            Preferences[key] = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public static void Set(string key, bool value)
        {
            Preferences[key] = Convert.ToString(value);
        }

    }
}
