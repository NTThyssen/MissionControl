using System;
namespace MissionControl.Data.Components
{
    public abstract class MeasuredComponent : Component
    {
        public delegate float Scaler(float val);
        public abstract int ByteSize { get; }
        public abstract bool Signed { get; }
        public abstract int Raw { get; }
        
        protected MeasuredComponent(byte boardID, string graphicID, string name ) : base(boardID, graphicID, name)
        {
        }

        abstract public void Set(int val);
    }
}
