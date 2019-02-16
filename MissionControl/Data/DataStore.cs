using System;
using System.Collections.Generic;
using System.Globalization;
using MissionControl.Data.Components;

namespace MissionControl.Data
{

    public interface IDataStore
    {   
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
            LoadSession();
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

        public Session GetCurrentSession()
        {
            return _session;
        }

        private void LoadSession() {

            _session.LogFilePath = PreferenceManager.Preferences[PreferenceManager.STD_LOGFOLDER] ?? _session.LogFilePath;
            _session.PortName = PreferenceManager.Preferences[PreferenceManager.STD_PORTNAME] ?? _session.PortName;

            string savedBaudRate = PreferenceManager.Preferences[PreferenceManager.STD_BAUDRATE];
            if (savedBaudRate != null)
            {
                if (int.TryParse(savedBaudRate, out int baud))
                {
                    _session.BaudRate = baud;
                }
            }

            foreach (Component c in _session.Mapping.Components())
            {

                string enabled = PreferenceManager.Preferences[c.PrefEnabledName];
                if (enabled != null)
                {
                    try { c.Enabled = Convert.ToBoolean(enabled); }
                    catch (Exception e) { Console.WriteLine("Setting enabled on {0} resulted in {1}", c.Name, e.Message); }
                }

                if (c is SensorComponent sc)
                {
                    string min = PreferenceManager.Preferences[sc.PrefMinName];
                    if (min != null)
                    {
                        try { sc.MinLimit = Convert.ToSingle(min); }
                        catch (Exception e) { Console.WriteLine("Setting min limit on {0} resulted in {1}", sc.Name, e.Message); }
                    }

                    string max = PreferenceManager.Preferences[sc.PrefMaxName];
                    if (max != null)
                    {
                        try { sc.MaxLimit = Convert.ToSingle(max); }
                        catch (Exception e) { Console.WriteLine("Setting max limit on {0} resulted in {1}", sc.Name, e.Message); }
                    }
                }
            }
        }


        public void UpdateSession(Session nsession)
        {
            PreferenceManager.Preferences[PreferenceManager.STD_LOGFOLDER] = nsession.LogFilePath;
            PreferenceManager.Preferences[PreferenceManager.STD_PORTNAME] = nsession.PortName;
            PreferenceManager.Preferences[PreferenceManager.STD_BAUDRATE] = nsession.BaudRate.ToString();

            _session.LogFilePath = nsession.LogFilePath;
            _session.PortName = nsession.PortName;
            _session.BaudRate = nsession.BaudRate;

            // Update Old Component with settings from New Component
            foreach (Component nc in nsession.Mapping.Components())
            {

                Component oc = _session.Mapping.ComponentByIDs()[nc.BoardID];

                PreferenceManager.Preferences[nc.PrefEnabledName] = Convert.ToString(nc.Enabled);
                oc.Enabled = nc.Enabled;

                if (nc is SensorComponent nsc)
                {
                    if (!float.IsNaN(nsc.MinLimit))
                    {
                        PreferenceManager.Preferences[nsc.PrefMinName] = nsc.MinLimit.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        PreferenceManager.Preferences.Remove(nsc.PrefMinName);
                    }
                    if (!float.IsNaN(nsc.MaxLimit))
                    {
                        PreferenceManager.Preferences[nsc.PrefMaxName] = nsc.MaxLimit.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        PreferenceManager.Preferences.Remove(nsc.PrefMaxName);
                    }

                    if (oc is SensorComponent _sc)
                    {
                        _sc.MinLimit = nsc.MinLimit;
                        _sc.MaxLimit = nsc.MaxLimit;
                    }
                }
            }

         
            PreferenceManager.Preferences.Save();

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
