using System;
namespace MissionControl.Data.Components
{
    public class ServoComponent : ValveComponent
    {
        private int _rawPosition;
        private float _closePosition;
        private float _openPosition;

        public ServoComponent(int boardID, string graphicID, string name, float closePosition, float openPosition, string graphicIDSymbol) : base(boardID, graphicID, name, graphicIDSymbol)
        {
            _closePosition = closePosition;
            _openPosition = openPosition;
        }

        public ServoComponent(int boardID, string graphicID, string name, string graphicIDSymbol) : base(boardID, graphicID, name, graphicIDSymbol)
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

        public float ClosePosition { get { return _closePosition; } }
        public float OpenPosition { get { return _openPosition; } }
    }
}
