using System;
using System.Collections.Generic;

namespace MissionControl.Data
{
    public class DataStore
    {
        private Queue<DataPacket> _data;
        private Session _session;

        public DataStore()
        {
            _data = new Queue<DataPacket>();
        }

        public void Enqueue(DataPacket packet) {
            _data.Enqueue(packet);
        }

        public DataPacket Dequeue() {
            return _data.Dequeue();
        }

        public Session CurrentSession() 
        {
            return _session;
        }
    }
}
