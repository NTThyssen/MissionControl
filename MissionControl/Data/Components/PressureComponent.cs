using System;
namespace MissionControl.Data.Components
{
    public class PressureComponent : Component
    {
        private int _rawPressure = 0;
        private Scaler _scaler;

        public PressureComponent(int boardID, string graphicID, string name, Scaler scaler) : base(boardID, graphicID, name)
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
    }
}
