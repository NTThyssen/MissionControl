using System;
namespace MissionControl.Data.Components
{
    public class ServoComponent : Component
    {
        private int _rawPosition;
        private float _closePosition;
        private float _openPosition;

        public ServoComponent(int boardID, string graphicID, string name, float closePosition, float openPosition) : base(boardID, graphicID, name)
        {
            _closePosition = closePosition;
            _openPosition = openPosition;
        }

        public ServoComponent(int boardID, string graphicID, string name) : base(boardID, graphicID, name)
        {
            _closePosition = 100.0f;
            _openPosition = 0.0f;
        }

        public override void Set(int val)
        {
            _rawPosition = val;
        }

        public float Degree()
        {
            return (_rawPosition / 100.0f) * 360;
        }

        public float Percentage()
        {
            return _rawPosition;
        }
    }
}
