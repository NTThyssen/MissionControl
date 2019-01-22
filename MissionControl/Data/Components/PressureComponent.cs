using System;
namespace MissionControl.Data.Components
{
    public class PressureComponent : Component, ILoggable
    {
        private int _rawPressure = 0;
        private Scaler _scaler;

        public override string TypeName => "Pressure";

        public PressureComponent(byte boardID, int byteSize, string graphicID, string name, Scaler scaler) : base(boardID, byteSize, graphicID, name)
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
            // Rounding to two decimals
            return "" + Math.Floor(Relative() * 100) / 100;
        }

        public string LogHeader()
        {
            return Name + " [barR]";
        }
    }
}
