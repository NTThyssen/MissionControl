using System;
namespace MissionControl.Data.Components
{
    public class SolenoidComponent : Component
    {
        public enum SolenoidState 
        {
            OPEN, 
            CLOSED
        };

        private int _raw;

        public SolenoidComponent(int boardID, string graphicID, string name) : base(boardID, graphicID, name)
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
    }
}
