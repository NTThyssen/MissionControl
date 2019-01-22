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

        public int MaxLimit { get; set; }
        public int MinLimit { get; set; }
        public bool HasLimits { get { return MinLimit != MaxLimit; } }
        public bool Enabled { get; set; } = true;

        public const string PREF_MAX_LIMIT = "_MAX_LIMIT";
        public const string PREF_MIN_LIMIT = "_MIN_LIMIT";
        public const string PREF_ENABLED = "_ENABLED";

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
