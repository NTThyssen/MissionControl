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
            
            Manager.Preferences.System.UseSerial = updated.System.UseSerial;
            Manager.Preferences.System.LogFilePath = updated.System.LogFilePath;
            
            Manager.Preferences.System.Serial.BaudRate = updated.System.Serial.BaudRate;
            Manager.Preferences.System.Serial.PortName = updated.System.Serial.PortName;
            
            Manager.Preferences.System.Ethernet.Port = updated.System.Ethernet.Port;
            Manager.Preferences.System.Ethernet.IPAddress = updated.System.Ethernet.IPAddress;

            Manager.Preferences.Fluid.Fuel.Density = updated.Fluid.Fuel.Density;
            Manager.Preferences.Fluid.Fuel.CV = updated.Fluid.Fuel.CV;
            
            Manager.Preferences.Fluid.Oxid.Density = updated.Fluid.Oxid.Density;
            Manager.Preferences.Fluid.Oxid.CV = updated.Fluid.Oxid.CV;

            Manager.Preferences.Fluid.TodaysPressure = updated.Fluid.TodaysPressure;

            Manager.Preferences.Visual.ShowAbsolutePressure = updated.Visual.ShowAbsolutePressure;
            Manager.Preferences.Visual.ShowSettingsOnStartup = updated.Visual.ShowSettingsOnStartup;
            
            foreach (KeyValuePair<byte, SensorSettings> kv in updated.Visual.SensorVisuals)
            {
                Manager.Preferences.Visual.SensorVisuals[kv.Key] = kv.Value;
            }
            
            Manager.Save();
            
        }

    }
}
