namespace MissionControl.Data.Components
{
    public class TankComponent : ComputedComponent, ILoggable
    {
        private FlowComponent _flow;
        private float _inputtedVolume = 1;
        private float _accumulated;
        
        private long _lastTime = 0;
        private float _lastMassFlow = 0;
        public string SettingsConstantName { get; }
        public float CurrentVolume { get; private set; }
        public string GraphicIDGradient { get; }
        
        public TankComponent(byte boardID, string graphicID, string name, string graphicIDGradient, ref FlowComponent flow,  string settingsConstantName) : base(boardID, graphicID, name)
        {
            _flow = flow;
            SettingsConstantName = settingsConstantName;
            GraphicIDGradient = graphicIDGradient;
        }

        public override string TypeName => "Tank";
        public override string ToDisplay()
        {
            return ToRounded(ToPercentage(), 2) + "%";
        }

        public float ToPercentage()
        {
            return CurrentVolume / _inputtedVolume * 100.0f;
        }

        public string ToLog()
        {
            return ToRounded(CurrentVolume, 3);
        }

        public string LogHeader()
        {
            return Name + " [L]";
        }

        public void Compute(long time)
        {
            long timeDelta = time - _lastTime;
            _accumulated += (timeDelta / 1000.0f) * _lastMassFlow;

            CurrentVolume = _inputtedVolume - _accumulated;
            
            _lastTime = time;
            _lastMassFlow = _flow.MassFlow;

        }

        public void SetInputVolume(long time, float volume)
        {
            _inputtedVolume = volume;
            _lastTime = time;
            _accumulated = 0;
            CurrentVolume = volume;
        }
    }
}