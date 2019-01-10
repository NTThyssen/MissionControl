using System;
using System.Collections.Generic;

namespace MissionControl.Data.Components
{
    public abstract class Component
    {
        public delegate float Scaler(float val);

        private int _boardID;
        private string _graphicID;
        private string _name;

        protected Component(int boardID, string graphicID, string name)
        {
            _boardID = boardID;
            _graphicID = graphicID;
            _name = name;
        }

        abstract public void Set(int val);

        public String GraphicID { get { return _graphicID; } }
        public String Name { get { return _graphicID; } }
    }
}
