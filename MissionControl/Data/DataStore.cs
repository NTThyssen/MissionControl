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
       
            // System
            _session.Setting.LogFilePath.Value = PreferenceManager.GetIfExists(_session.Setting.LogFilePath.PreferenceKey, _session.Setting.LogFilePath.Value);
            _session.Setting.PortName.Value = PreferenceManager.GetIfExists(_session.Setting.PortName.PreferenceKey, _session.Setting.PortName.Value);
            _session.Setting.BaudRate.Value = PreferenceManager.GetIfExists(_session.Setting.BaudRate.PreferenceKey, 9600);

            // Fluid
            _session.Setting.OxidCV.Value = PreferenceManager.GetIfExists(_session.Setting.OxidCV.PreferenceKey, 1.0f);
            _session.Setting.OxidGL.Value = PreferenceManager.GetIfExists(_session.Setting.OxidGL.PreferenceKey, 1.0f);
            _session.Setting.FuelCV.Value = PreferenceManager.GetIfExists(_session.Setting.FuelCV.PreferenceKey, 1.0f);
            _session.Setting.FuelGL.Value = PreferenceManager.GetIfExists(_session.Setting.FuelGL.PreferenceKey, 1.0f);
            _session.Setting.TodayPressure.Value = PreferenceManager.GetIfExists(_session.Setting.TodayPressure.PreferenceKey, 1.0f);
            _session.Setting.ShowAbsolutePressure.Value = PreferenceManager.GetIfExists(_session.Setting.ShowAbsolutePressure.PreferenceKey, false);

            foreach (Component c in _session.Mapping.Components())
            {
                c.Enabled = PreferenceManager.GetIfExists(c.PrefEnabledName, c.Enabled);

                if (c is SensorComponent sc)
                {
                    sc.MinLimit = PreferenceManager.GetIfExists(sc.PrefMinName, sc.MinLimit);
                    sc.MaxLimit = PreferenceManager.GetIfExists(sc.PrefMinName, sc.MaxLimit);
                }
            }
        }


        public void UpdateSession(Session nsession)
        {

            try
            {
                List<Property> oldProps = _session.Setting.Properties();
                List<Property> newProps = nsession.Setting.Properties();

                for (int i = 0; i < newProps.Count; i++)
                {

                    switch (oldProps[i])
                    {
                        case StringProperty p:
                            PreferenceManager.Set(newProps[i].PreferenceKey, p.ToString());
                            p.Value = (newProps[i] as StringProperty).Value;
                            break;
                        case IntegerProperty p:
                            PreferenceManager.Set(newProps[i].PreferenceKey, p.ToString());
                            p.Value = (newProps[i] as IntegerProperty).Value;
                            break;
                        case FloatProperty p:
                            PreferenceManager.Set(newProps[i].PreferenceKey, p.ToString());
                            p.Value = (newProps[i] as FloatProperty).Value;
                            break;
                        case BoolProperty p:
                            PreferenceManager.Set(newProps[i].PreferenceKey, p.ToString());
                            p.Value = (newProps[i] as BoolProperty).Value;
                            break;
                    }
                }
            } catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Old and new settings does not have the same length of properties");
            }

                  
            // Update Old Component with settings from New Component
            foreach (Component nc in nsession.Mapping.Components())
            {

                Component oc = _session.Mapping.ComponentByIDs()[nc.BoardID];

                PreferenceManager.Set(nc.PrefEnabledName, nc.Enabled);
                oc.Enabled = nc.Enabled;

                if (nc is SensorComponent nsc)
                {
                    if (!float.IsNaN(nsc.MinLimit))
                    {
                        PreferenceManager.Set(nsc.PrefMinName, nsc.MinLimit);
                    }
                    else
                    {
                        PreferenceManager.Preferences.Remove(nsc.PrefMinName);
                    }
                    if (!float.IsNaN(nsc.MaxLimit))
                    {
                        PreferenceManager.Set(nsc.PrefMaxName, nsc.MaxLimit);
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

        private void UpdateSetting(ref Property o, Property n)
        {
            PreferenceManager.Preferences[n.PreferenceKey] = n.ToString();
            o = n;
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
