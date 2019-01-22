using System;
namespace MissionControl.Data.Components
{
    public class VoltageComponent : Component, ILoggable
    {

        private float _minVoltage;
        private float _maxVoltage;
        private int _rawVoltage;

        public override string TypeName => "Voltage";

        public VoltageComponent(byte boardID, int byteSize, string graphicID, string name, float minVoltage, float maxVoltage) : base(boardID, byteSize, graphicID, name)
        {
            // Guard
            _minVoltage = Math.Min(minVoltage, maxVoltage);
            _maxVoltage = Math.Max(minVoltage, maxVoltage);
        }

        public override void Set(int val)
        {
            _rawVoltage = val;
        }

        public float Percentage()
        {
            return ((_rawVoltage - _minVoltage) / (_maxVoltage - _minVoltage)) * 100;
        }

        public float Volts()
        {
            return _rawVoltage;
        }

        public string ToLog()
        {
            // Rounding to two decimals
            return "" + Math.Floor(Volts() * 100) / 100;
        }

        public string LogHeader()
        {
            return Name + " [V]";
        }
    }
}
