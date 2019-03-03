using System;
using System.Globalization;
using Pango;

namespace MissionControl.Data.Components
{
    public class LoadComponent : SensorComponent, ILoggable
    {
        private int _rawLoad;
        private Scaler _scaler;
        private int _taredValue;
      

        public override string TypeName => "Load cell";
        public override int ByteSize => 2;
        public override bool Signed => true;

        public float Gravity => 10.0f; 

        public LoadComponent(byte boardID, string graphicID, string name, Scaler scaler) : base(boardID, graphicID, name)
        {
            _scaler = scaler;
        }
        
        public override void Set(int val)
        {
            _rawLoad = val;
        }

        public float Newtons()
        {
            return _scaler(_rawLoad - _taredValue) * Gravity;
        }

        public float Kilos()
        {
            return _scaler(_rawLoad);
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
