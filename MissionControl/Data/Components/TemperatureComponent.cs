using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class TemperatureComponent : SensorComponent, ILoggable
    {
        private int _rawTemperature = 0;
        private Calibrator _calibrator;

        public override string TypeName => "Temperature";
        public override int ByteSize => 2;
        public override bool Signed => true;
        public override int Raw => _rawTemperature;

        public TemperatureComponent(byte boardID, string graphicID, string name, Calibrator calibrator) : base(boardID, graphicID, name)
        {
            _calibrator = calibrator;
        }

        public float Kelvin()
        {
            return _calibrator(_rawTemperature) + 271.15f;
        }

        public float Celcius()
        {
            return _calibrator(_rawTemperature);
        }

        public override void Set(int val)
        {
            _rawTemperature = val;
        }

        public string ToLog()
        {
            // Rounding to one decimal
            return (Math.Floor(Celcius() * 10) / 10).ToString(CultureInfo.InvariantCulture); ;
        }

        public string LogHeader()
        {
            return Name + " [°C]";
        }

        public override string ToDisplay()
        {
            return ToRounded(Celcius(), 1);
        }
    }
}
