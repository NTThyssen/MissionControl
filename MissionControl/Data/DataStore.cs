using System;
using System.Collections.Generic;

namespace MissionControl.Data
{

    public interface IDataStore
    {   
        void GetCurrentState();
        Session GetCurrentSession();
        void UpdateSession(Session session);
        void EnableLogging();
        void DisableLogging();
    }

    public interface IDataLog
    {
        void Enqueue(DataPacket packet);
        DataPacket Dequeue();
        bool Empty();
        Session GetCurrentSession();
    }

    public class DataStore : IDataStore, IDataLog
    {
        private Queue<DataPacket> _data;
        private Session _session;
       
        private bool _isLogging = false;

        public DataStore(Session session)
        {
            _data = new Queue<DataPacket>();
            _session = session;
        }

        public void Enqueue(DataPacket packet) {
            _session.LastReceived = packet.ReceivedTime;
            if (_isLogging)
            {
                _data.Enqueue(packet);
            }
            else
            {
                _session.UpdateComponents(packet.Bytes);
            }
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

        public void UpdateSession(Session session)
        {
            _session.LogFilePath = session.LogFilePath;
            _session.PortName = session.PortName;
            // Might have trouble with mapping
        }

        public void EnableLogging()
        {
            _isLogging = true;
        }

        public void DisableLogging()
        {
            _isLogging = false;
        }

    }
}
