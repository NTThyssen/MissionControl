using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class PressureComponent : SensorComponent, ILoggable
    {
        private int _rawPressure = 0;
        private Scaler _scaler;

        public override string TypeName => "Pressure";
        public override int ByteSize => 2;
        public override bool Signed => false;

        public PressureComponent(byte boardID, string graphicID, string name, Scaler scaler) : base(boardID, graphicID, name)
        {
            _scaler = scaler;
        }

        public float Relative()
        {
            return _scaler(_rawPressure);
        }

        public float Absolute(float atmosphere) 
        { 
            return _scaler(_rawPressure) + atmosphere;
        }

        public override void Set(int val)
        {
            _rawPressure = val;
        }

        public string ToLog()
        {
            return ToRounded(Relative(), 4);
        }

        public override string ToDisplay()
        {
            return ToRounded(Relative(), 2);
        }

        public string LogHeader()
        {
            return Name + " [barg]";
        }
    }
}
