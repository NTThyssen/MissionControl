using System.Collections.Generic;
using MissionControl.Connection.Commands;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public class Preferences
    {
        public SystemSettings System { get; } = new SystemSettings();
        public FluidSettings Fluid { get; } = new FluidSettings();
        public VisualSettings Visual { get; } = new VisualSettings();
        
        public AutoParameters AutoSequence { get; set; } = new AutoParameters();
        public AutoSequenceComponentIDs AutoSequenceComponentIDs { get; set; } = new AutoSequenceComponentIDs();
    }

    public class FluidSettings
    {

        public Fluid Oxid { get; } = new Fluid {CV = 0.2323f, Density = 1980};
        public Fluid Fuel { get; } = new Fluid {CV = 0.0738f, Density = 786};
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
        public SerialSettings Serial { get; } = new SerialSettings();
        public EthernetSettings Ethernet { get; } = new EthernetSettings();
    }

    public class VisualSettings
    {
        public bool ShowSettingsOnStartup { get; set; } = true;
        public bool ShowAbsolutePressure { get; set; } = true;
        public Dictionary<byte, SensorSettings> SensorVisuals { get; } = new Dictionary<byte, SensorSettings>();
    }

    public class AutoSequenceComponentIDs
    {
        public byte ChamberPressureID { get; set; }
        public byte FuelLinePressureID { get; set; }
        public byte OxidizerLinePressureID { get; set; }
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