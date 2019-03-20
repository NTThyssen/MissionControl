using System;
namespace MissionControl.Data.Components
{
    public abstract class ValveComponent : MeasuredComponent
    {
        private readonly string _graphicIDSymbol;

        protected ValveComponent(byte boardID, string graphicID, string name, string graphicIDSymbol) : base(boardID, graphicID, name)
        {
            _graphicIDSymbol = graphicIDSymbol;
        }

        public string GraphicIDSymbol => _graphicIDSymbol;
    }

}
