using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class TankComponent : MeasuredComponent, ILoggable
    {

        private readonly float _full;
        private readonly float _initial;
        private int _rawVolume;
        private readonly string _graphicIDGradient;
        
        public override int ByteSize => 2;
        public override bool Signed => false;

        public TankComponent(byte boardID, string graphicID, string name, string graphicIDGradient, float full, float initial) : base(boardID, graphicID, name)
        {
            _graphicIDGradient = graphicIDGradient;
            _full = full;
            _initial = initial;
        }


        public override void Set(int val)
        {
            _rawVolume = val;
        }

        public void Decrement(int val)
        {
            _rawVolume -= val;
        }

        public String GraphicIDGradient => _graphicIDGradient;
        public float Full => _full;
        public float Initial => _initial;

        public override string TypeName => "Tank";

        public float PercentageInit() 
        {
            return (_rawVolume / _initial) * 100;
        }

        public float PercentageFull()
        {
            return (_rawVolume / _full) * 100;
         }

        public float Litres()
        {
            return _rawVolume / 1000.0f;
        }

        public string ToLog()
        {
            // Rounding to three decimals
            return ToRounded(Litres(), 4);
        }

        public string LogHeader()
        {
            return Name + " [L]";
        }

        public override string ToDisplay()
        {
            return ToRounded(Litres(), 2);
        }
    }
}
