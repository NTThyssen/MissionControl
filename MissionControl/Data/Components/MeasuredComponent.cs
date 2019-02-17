using System;
namespace MissionControl.Data.Components
{
    public abstract class MeasuredComponent : Component
    {
        public delegate float Scaler(float val);
        public int ByteSize { get; }


        protected MeasuredComponent(byte boardID, int byteSize, string graphicID, string name ) : base(boardID, graphicID, name)
        {
            ByteSize = byteSize;
        }

        abstract public void Set(int val);
    }
}
