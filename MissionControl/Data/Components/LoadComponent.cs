using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class LoadComponent : SensorComponent, ILoggable
    {
        private int _rawLoad;
        private Scaler _scaler;
        private float _taredValue;
      

        public override string TypeName => "Load cell";
        public override int ByteSize => 2;
        public override bool Signed => true;

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
            
            return _scaler(_rawLoad) * 10.0f - _taredValue;
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
            _taredValue = _scaler(_rawLoad) * 10.0f;

        }

        public override string ToDisplay()
        {
            return ToRounded(Newtons(), 2);
        }
    }
}
