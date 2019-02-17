using System;
namespace MissionControl.Data.Components
{
    public abstract class ComputedComponent : Component
    {
        protected ComputedComponent(byte boardID, string graphicID, string name) : base(boardID, graphicID, name)
        {
        }
      
       
    }
}
