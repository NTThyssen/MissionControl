namespace MissionControl.Data.Components
{
    public class SimpleComponent : MeasuredComponent, ILoggable
    {
        
        private int _rawPosition;
        private readonly string _logHeader;

        private Calibrator _calibrator;
        
        public SimpleComponent(byte boardID, int byteSize, bool signed, string name, string logHeader, Calibrator calibrator) : base(boardID, null, name)
        {
            _logHeader = logHeader;
            Signed = signed;
            ByteSize = byteSize;
            _calibrator = calibrator;
        }

        public override string TypeName => "SimpleComponent";

        public override int ByteSize { get; }

        public override bool Signed { get; }

        public override int Raw => _rawPosition;

        public override void Set(int val)
        {
            _rawPosition = val;
        }

        public string ToLog()
        {
            return ToRounded(_calibrator(_rawPosition), 1);
        }

        public string LogHeader()
        {
            return _logHeader;
        }

        public override string ToDisplay()
        {
            return ToRounded(_calibrator(_rawPosition), 1);
        }
    }
}