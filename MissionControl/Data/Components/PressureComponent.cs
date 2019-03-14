using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class PressureComponent : SensorComponent, ILoggable
    {
        private int _rawPressure = 373;

        private const int _minADC = 373;
        private const int _maxADC = 1862;
        private static int _maxPressure = 50;

        private  Scaler DefaultScaler = x => { return _maxPressure * (x - _minADC) / (_maxADC - _minADC); };
        //private float Calibrated => _maxPressure * (_rawPressure - _minADC) / (_maxADC - _minADC);
        private readonly Scaler _scaler;
        private float Calibrated => _scaler(_rawPressure);
        
        public override string TypeName => "Pressure";
        public override int ByteSize => 2;
        public override bool Signed => false;
        public override int Raw => _rawPressure;

        public PressureComponent(byte boardID, string graphicID, string name, int maxPressure, Scaler scaler) : base(boardID, graphicID, name)
        {
            _maxPressure = maxPressure;
            _scaler = scaler ?? DefaultScaler;
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
