using System;
namespace MissionControl.Data
{
    public class DataPacket
    {
        public DateTime ReceivedTime{ get; }
        public byte[] Bytes { get;}

        public DataPacket(byte[] data)
        {
            ReceivedTime = DateTime.Now;
            Bytes = data;
        }
    }
}
