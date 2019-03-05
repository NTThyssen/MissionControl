using System.Collections.Generic;
using MissionControl.Connection.Commands;

namespace MissionControl.Data
{
    public class Preferences
    {
        public SystemSettings System { get; } = new SystemSettings();
        public FluidSettings Fluid { get; } = new FluidSettings();
        public VisualSettings Visual { get; } = new VisualSettings();
        
        public AutoParameters AutoSequence { get; set; } = new AutoParameters();
        
        public bool[] Bools => new bool[]{};
        public float[] Floats => new float[]{};
        public string[] Strings => new string[]{};
        public int[] Integers => new int[]{};
    }

    public class FluidSettings
    {

        public Fluid Oxid { get; set; } = new Fluid {CV = 0.2323f, Density = 1980};
        public Fluid Fuel { get; set; } = new Fluid {CV = 0.0738f, Density = 786};
        public float TodaysPressure { get; set; } = 1.020f;
    }

    public class Fluid
    {
        public float CV { get; set; }
        public float Density { get; set; }
    } 

    public class SystemSettings
    {
        public string LogFilePath  { get; set; }
        public bool UseSerial { get; set; } = true;
        public SerialSettings Serial { get; set; } = new SerialSettings();
        public EthernetSettings Ethernet { get; set; } = new EthernetSettings();
    }

    public class VisualSettings
    {
        public bool ShowSettingsOnStartup  { get; set; }
        public bool ShowAbsolutePressure  { get; set; }
        public Dictionary<byte, SensorSettings> SensorVisuals { get; set; } = new Dictionary<byte, SensorSettings>();
    }

    public class SensorSettings
    {
        public float Min { get; set; } = float.NaN;
        public float Max { get; set; } = float.NaN;
        public bool Enabled { get; set; } = true;
    }

    public class SerialSettings
    {
        public string PortName  { get; set; }
        public int BaudRate { get; set; } = 9600;
    }
    
    public class EthernetSettings
    {
        public string IPAddress  { get; set; }
        public string Port  { get; set; }
    }
}