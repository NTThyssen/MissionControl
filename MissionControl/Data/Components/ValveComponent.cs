using System;
namespace MissionControl.Data.Components
{
    public abstract class ValveComponent : Component
    {
        private readonly string _graphicIDSymbol;

        protected ValveComponent(int boardID, string graphicID, string name, string graphicIDSymbol) : base(boardID, graphicID, name)
        {
            _graphicIDSymbol = graphicIDSymbol;
        }

        public String GraphicIDSymbol { get { return _graphicIDSymbol; } }
    }

}
