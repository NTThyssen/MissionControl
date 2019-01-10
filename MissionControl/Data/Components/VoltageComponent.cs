using System;
namespace MissionControl.Data.Components
{
    public class VoltageComponent : Component
    {

        private float _minVoltage;
        private float _maxVoltage;
        private int _rawVoltage;

        public VoltageComponent(int boardID, string graphicID, string name, float full, float initial, float minVoltage, float maxVoltage) : base(boardID, graphicID, name)
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
            return (_rawVoltage - _minVoltage) / (_maxVoltage - _minVoltage);
        }

        public float Volts()
        {
            return _rawVoltage;
        }
    }
}
