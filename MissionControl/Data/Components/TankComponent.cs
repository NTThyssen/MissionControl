using System;
namespace MissionControl.Data.Components
{
    public class TankComponent : Component
    {

        private float _full;
        private float _initial;
        private int _rawVolume;
        private string _graphicIDGradient;

        public TankComponent(int boardID, string graphicID, string name, string graphicIDGradient, float full, float initial) : base(boardID, graphicID, name)
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
            return _rawVolume / 1000;
        }
    }
}
