using System;
namespace MissionControl.Data.Components
{
    public class LoadComponent : SensorComponent, ILoggable
    {
        private int _rawLoad;
        private Scaler _scaler;

        public override string TypeName => "Load cell";

        public LoadComponent(byte boardID, int byteSize, string graphicID, string name, Scaler scaler) : base(boardID, byteSize, graphicID, name)
        {
            _scaler = scaler;
        }

        public override void Set(int val)
        {
            _rawLoad = val;
        }

        public float Newtons()
        {
            return _scaler(_rawLoad) * 10.0f;
        }

        public float Kilos()
        {
            return _scaler(_rawLoad);
        }

        public string ToLog()
        {
            return Newtons() + "";
        }

        public string LogHeader()
        {
            return Name + " [N]";
        }
    }
}
