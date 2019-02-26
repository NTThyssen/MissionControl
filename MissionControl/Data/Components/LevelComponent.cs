using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class LevelComponent : MeasuredComponent, ILoggable
    {
        private readonly float _initial;
        private int _rawVolume;

        public override int ByteSize => 2;
        public override bool Signed => false;
        public String GraphicIDGradient { get; }
        public float Total { get; }
        
        public LevelComponent(byte boardID, string graphicID, string name, string graphicIDGradient, float total) : base(boardID, graphicID, name)
        {
            GraphicIDGradient = graphicIDGradient;
            Total = total;
        }

        public override void Set(int val)
        {
            _rawVolume = val;
        }

        public override string TypeName => "Level";

        public float PercentageFull()
        {
            return (_rawVolume / Total) * 100;
         }

        public float Litres()
        {
            return _rawVolume / 1000.0f;
        }

        public string ToLog()
        {
            return ToRounded(Litres(), 4);
        }

        public string LogHeader()
        {
            return Name + " [L]";
        }

        public override string ToDisplay()
        {
            return ToRounded(PercentageFull(), 2);
        }
    }
}
