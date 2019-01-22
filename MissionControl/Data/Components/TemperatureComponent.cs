using System;
namespace MissionControl.Data.Components
{
    public class TemperatureComponent : SensorComponent, ILoggable
    {
        private int _rawTemperature = 0;
        private Scaler _scaler;

        public override string TypeName => "Temperature";

        public TemperatureComponent(byte boardID, int byteSize, string graphicID, string name, Scaler scaler) : base(boardID, byteSize, graphicID, name)
        {
            _scaler = scaler;
        }

        public float Kelvin()
        {
            return _scaler(_rawTemperature) + 271.15f;
        }

        public float Celcius()
        {
            return _scaler(_rawTemperature);
        }

        public override void Set(int val)
        {
            _rawTemperature = val;
        }

        public string ToLog()
        {
            // Rounding to one decimal
            return "" + Math.Floor(Celcius() * 10) / 10;
        }

        public string LogHeader()
        {
            return Name + " [°C]";
        }
    
    }
}
