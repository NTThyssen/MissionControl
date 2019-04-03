using System;
using System.Globalization;
using Pango;

namespace MissionControl.Data.Components
{
    public class LoadComponent : SensorComponent, ILoggable
    {
        private int _rawLoad = 0;
        private int _taredValue;
        
        public float Gravity => 9.82f;
        private float _mvPrBit = (float) (3.3f / Math.Pow(2,12)) * 1000;
        private float _mvPrGram = ((3.0008f * 10) / 500000) * 70;
        
        private float Calibrated => (_rawLoad - _taredValue) * (_mvPrBit / _mvPrGram) * (Gravity / 1000);

        public override string TypeName => "Load cell";
        public override int ByteSize => 2;
        public override bool Signed => true;
        public override int Raw => _rawLoad;

        public LoadComponent(byte boardID, string graphicID, string name) : base(boardID, graphicID, name)
        {
        }
        
        public override void Set(int val)
        {
            _rawLoad = val;
        }
        
        public float Newtons()
        {
            return Calibrated;
        }

        public float Kilos()
        {
            return Calibrated / Gravity;
        }

        public string ToLog()
        {
            return ToRounded(Newtons(), 6);
        }

        public string LogHeader()
        {
            return Name + " [N]";
        }

        public void Tare()
        {
            _taredValue = _rawLoad;

        }

        public override string ToDisplay()
        {
            return ToRounded(Newtons(), 2);
        }
    }
}
