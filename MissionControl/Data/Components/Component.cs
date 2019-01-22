using System;
using System.Collections.Generic;

namespace MissionControl.Data.Components
{
    public abstract class Component
    {
        public delegate float Scaler(float val);

        public string GraphicID { get; }
        public string Name { get; }
        public int ByteSize { get; }
        public byte BoardID { get; }

        public bool Enabled { get; set; } = true;

        public string PrefEnabledName { get { return BoardID + "_ENABLED"; } }

        public abstract string TypeName { get; }

        protected Component(byte boardID, int byteSize, string graphicID, string name)
        {
            BoardID = boardID;
            ByteSize = byteSize;
            GraphicID = graphicID;
            Name = name;
        }

        abstract public void Set(int val);

      }
}
