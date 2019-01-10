using System;
namespace MissionControl.Data.Components
{
    public class TemperatureComponent : Component
    {
        private int _rawTemperature = 0;
        private Scaler _scaler;

      public TemperatureComponent(int boardID, string graphicID, string name, Scaler scaler) : base(boardID, graphicID, name)
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
    }
}
