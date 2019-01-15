using System;
using System.Collections.Generic;

namespace MissionControl.Data
{

    public interface IDataStore
    {   
        void GetCurrentState();
        Session GetCurrentSession();
    }

    public interface IDataLog
    {
        void Enqueue(DataPacket packet);
        DataPacket Dequeue();
        bool Empty();
    }

    public class DataStore : IDataStore, IDataLog
    {
        private Queue<DataPacket> _data;
        private Session _session;
        private LogThread _logThread;

        private bool _isLogging = false;

        public DataStore(Session session)
        {
            _data = new Queue<DataPacket>();
            _session = session;
            _logThread = new LogThread(this);
        }

        public void Enqueue(DataPacket packet) {
            _data.Enqueue(packet);
        }

        public DataPacket Dequeue() {
            return _data.Dequeue();
        }

        public bool Empty()
        {
            return _data.Count == 0;
        }

        public void GetCurrentState()
        {
            throw new NotImplementedException();
        }

        public Session GetCurrentSession()
        {
            return _session;
        }


    }
}
