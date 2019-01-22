using System;
namespace MissionControl.Data.Components
{
    public abstract class ValveComponent : Component
    {
        private readonly string _graphicIDSymbol;

        protected ValveComponent(byte boardID, int byteSize, string graphicID, string name, string graphicIDSymbol) : base(boardID, byteSize, graphicID, name)
        {
            _graphicIDSymbol = graphicIDSymbol;
        }

        public String GraphicIDSymbol { get { return _graphicIDSymbol; } }
    }

}
