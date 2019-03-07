namespace MissionControl.Data.Components
{
    public class ServoTargetComponent : MeasuredComponent, ILoggable
    {
        
        private int _rawPosition;
        
        public ServoTargetComponent(byte boardID, string name) : base(boardID, null, name)
        {
        }

        public override string TypeName => "ServoTarget";

        public override int ByteSize => 2;
        public override bool Signed => false;
        public override void Set(int val)
        {
            _rawPosition = val;
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