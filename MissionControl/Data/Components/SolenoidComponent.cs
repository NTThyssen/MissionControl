using System;
namespace MissionControl.Data.Components
{
    public class SolenoidComponent : ValveComponent, ILoggable
    {
        public enum SolenoidState 
        {
            OPEN, 
            CLOSED
        };

        private int _raw;

        public SolenoidComponent(byte boardID, int byteSize, string graphicID, string name, string graphicIDSymbol) : base(boardID, byteSize, graphicID, name, graphicIDSymbol)
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

        public string ToLog()
        {
            return "" + BinaryValue();
        }

        public string LogHeader()
        {
            return Name + "[OPEN/CLOSE]";
        }

        public bool Open { get { return _raw != 0; } }

        public override string TypeName => "Solenoid";
    }
}
