using System;
namespace MissionControl.Data.Components
{
    public class LoadComponent : Component
    {
        private int _rawLoad;
        private Scaler _scaler;

        public LoadComponent(int boardID, string graphicID, string name, Scaler scaler) : base(boardID, graphicID, name)
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
    }
}
