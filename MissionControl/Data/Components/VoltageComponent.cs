using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class VoltageComponent : SensorComponent, ILoggable
    {

        private float _minVoltage;
        private float _maxVoltage;
        private int _rawVoltage;

        public override string TypeName => "Voltage";
        public override int ByteSize => 2;
        public override bool Signed => false;
        public override int Raw => _rawVoltage;

        public VoltageComponent(byte boardID, string graphicID, string name, float minVoltage, float maxVoltage) : base(boardID, graphicID, name)
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
            return ToRounded(Volts(), 2);
        }

        public string LogHeader()
        {
            return Name + " [V]";
        }

        public override string ToDisplay()
        {
            return ToRounded(Volts(), 2);
        }
    }
}
