using System;
namespace MissionControl.Data.Components
{
    public class ServoComponent : ValveComponent, ILoggable
    {
        private int _rawPosition;

        public float ClosePosition { get; }
        public float OpenPosition { get; }

        public override string TypeName => "Servo";
        public override int ByteSize => 2;
        public override bool Signed => false;

        public ServoComponent(byte boardID, string graphicID, string name, float closePosition, float openPosition, string graphicIDSymbol) : base(boardID, graphicID, name, graphicIDSymbol)
        {
            ClosePosition = closePosition;
            OpenPosition = openPosition;
        }

        public ServoComponent(byte boardID, string graphicID, string name, string graphicIDSymbol) : base(boardID, graphicID, name, graphicIDSymbol)
        {
            ClosePosition = 0.0f;
            OpenPosition = 100.0f;
        }

        public override void Set(int val)
        {
            _rawPosition = val;
        }

        public float Degree()
        {
            return (Percentage() / 100.0f) * 360;
        }

        public float Percentage()
        {
            return ((float) _rawPosition) / ushort.MaxValue * 100.0f;
        }

        public string ToLog()
        {
            return ToRounded(Percentage(), 1);
        }

        public string LogHeader()
        {
            return Name + " [%]";
        }

        public override string ToDisplay()
        {
            return ToRounded(Percentage(), 1);
        }
    }
}
