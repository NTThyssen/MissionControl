using System;
namespace MissionControl.Data.Components
{
    public class SolenoidComponent : ValveComponent
    {
        public enum SolenoidState 
        {
            OPEN, 
            CLOSED
        };

        private int _raw;

        public SolenoidComponent(int boardID, string graphicID, string name, string graphicIDSymbol) : base(boardID, graphicID, name, graphicIDSymbol)
        {
        }

        public override void Set(int val)
        {
            _raw = val;
        }

        public SolenoidState State()
        {
            return (_raw == 0) ? SolenoidState.CLOSED : SolenoidState.OPEN;
        }

        public int BinaryValue() 
        {
            return (_raw == 0) ? 0 : 1;
        }

        public bool Open { get { return _raw != 0; } }
    }
}
