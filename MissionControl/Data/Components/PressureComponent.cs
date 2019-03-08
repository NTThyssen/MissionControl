using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class PressureComponent : SensorComponent, ILoggable
    {
        private int _rawPressure = 373;

        private readonly float _minADC = 372.36f;
        private readonly float _maxADC = 1861.81f;
        private readonly float _maxPressure;
        private float Calibrated => _maxPressure * (_rawPressure - _minADC) / (_maxADC - _minADC);
        
        public override string TypeName => "Pressure";
        public override int ByteSize => 2;
        public override bool Signed => false;

        public PressureComponent(byte boardID, string graphicID, string name, float maxPressure) : base(boardID, graphicID, name)
        {
            _maxPressure = maxPressure;
        }

        public float Relative()
        {
            return Calibrated;
        }

        public float Absolute(float atmosphere) 
        { 
            return Calibrated + atmosphere;
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
