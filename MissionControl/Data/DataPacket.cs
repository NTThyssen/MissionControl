using System;
namespace MissionControl.Data
{
    public class DataPacket
    {
        long Time;
        byte[] Data;

        public DataPacket(long time, byte[] data)
        {
            Time = time;
            Data = data;
        }
    }
}
