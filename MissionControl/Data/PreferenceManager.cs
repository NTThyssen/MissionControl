using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MissionControl.Data
{

    // Singleton pattern
    // http://csharpindepth.com/articles/general/singleton.aspx

    public sealed class PreferenceManager
    {
  
        private readonly string _filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"preferences.json");
        //private Dictionary<string, string> _preferences;
        private readonly Preferences _preferences;
        
        public static PreferenceManager Manager { get; } = new PreferenceManager();

        public Preferences Preferences => _preferences;

        /* public string this [string key]
        {
            get {
                return _preferences.ContainsKey(key) ? _preferences[key] : null;
            }
            set { _preferences[key] = value; }
        }*/

        static PreferenceManager()
        {
        }

        /*
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
        }*/
        
        public PreferenceManager()
        {
            
            //_preferences = new Dictionary<string, string>();
            _preferences = new Preferences();

            if (File.Exists(_filepath))
            {
                string file = File.ReadAllText(_filepath);
                _preferences = JsonConvert.DeserializeObject<Preferences>(file);
            }
            else
            {
                _preferences = new Preferences();
            }
        }

        /*public void Remove(string key)
        {
            if (_preferences.ContainsKey(key))
            {
                _preferences.Remove(key);
            }
        }*/

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(_filepath, false))
            {
                string text = JsonConvert.SerializeObject(_preferences);
                sw.Write(text);
                /*foreach (KeyValuePair<string, string> kv in _preferences)
                {
                    sw.WriteLine("{0}:{1}", kv.Key, kv.Value);
                }*/
            }
        }

        public static SensorSettings GetSensorSettings(byte key)
        {
            return Manager.Preferences.Visual.SensorVisuals.TryGetValue(key, out SensorSettings value) ? value : new SensorSettings();
        }

        public static void UpdatePreferences(Preferences updated)
        {
            for (int i = 0; i < updated.Bools.Length; i++) { Manager.Preferences.Bools[i] = updated.Bools[i]; }
            for (int i = 0; i < updated.Floats.Length; i++) { Manager.Preferences.Floats[i] = updated.Floats[i]; }
            for (int i = 0; i < updated.Strings.Length; i++) { Manager.Preferences.Strings[i] = updated.Strings[i]; }
            for (int i = 0; i < updated.Integers.Length; i++) { Manager.Preferences.Integers[i] = updated.Integers[i]; }

            foreach (KeyValuePair<byte, SensorSettings> kv in updated.Visual.SensorVisuals)
            {
                Manager.Preferences.Visual.SensorVisuals[kv.Key] = kv.Value;
            }
            
        }

        /*public static int GetIfExists(string key, int defaultValue)
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
        }*/

    }
}
