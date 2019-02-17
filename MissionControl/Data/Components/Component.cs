using System;
using System.Collections.Generic;

namespace MissionControl.Data.Components
{
    public abstract class Component
    {

        public byte BoardID { get; }
        public string GraphicID { get; }
        public string Name { get; }
        public bool Enabled { get; set; } = true;

        public string PrefEnabledName { get { return GraphicID + "_ENABLED"; } }

        public abstract string TypeName { get; }

        protected Component(byte boardID, string graphicID, string name)
        {
            BoardID = boardID;
            GraphicID = graphicID;
            Name = name;
        }



      }
}
