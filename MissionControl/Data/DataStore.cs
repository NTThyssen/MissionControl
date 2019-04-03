using System;
using System.Collections.Generic;
using System.Globalization;
using MissionControl.Data.Components;

namespace MissionControl.Data
{

    public interface IDataStore
    {   
        Session GetCurrentSession();
        void UpdateSession(Preferences preferences);
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
            _session.LastReceivedBytes = packet.Bytes;
            _session.QueueSize = _data.Count;
            if (_isLogging)
            {
                _data.Enqueue(packet);
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
       /*
            // System
            _session.Setting.LogFilePath.Value = PreferenceManager.GetIfExists(_session.Setting.LogFilePath.PreferenceKey, _session.Setting.LogFilePath.Value);
            _session.Setting.PortName.Value = PreferenceManager.GetIfExists(_session.Setting.PortName.PreferenceKey, _session.Setting.PortName.Value);
            _session.Setting.BaudRate.Value = PreferenceManager.GetIfExists(_session.Setting.BaudRate.PreferenceKey, 9600);
            _session.Setting.ShowSettingsOnStartup.Value = PreferenceManager.GetIfExists(_session.Setting.ShowSettingsOnStartup.PreferenceKey, true);

            // Auto Parameters
            _session.Setting.AutoParameters.Value =
                PreferenceManager.GetIfExists(_session.Setting.AutoParameters.PreferenceKey, "");
            
            // Fluid
            _session.Setting.OxidCV.Value = PreferenceManager.GetIfExists(_session.Setting.OxidCV.PreferenceKey, 1.0f);
            _session.Setting.OxidDensity.Value = PreferenceManager.GetIfExists(_session.Setting.OxidDensity.PreferenceKey, 1.0f);

            _session.Setting.FuelCV.Value = PreferenceManager.GetIfExists(_session.Setting.FuelCV.PreferenceKey, 1.0f);
            _session.Setting.FuelDensity.Value = PreferenceManager.GetIfExists(_session.Setting.FuelDensity.PreferenceKey, 1.0f);

            _session.Setting.TodayPressure.Value = PreferenceManager.GetIfExists(_session.Setting.TodayPressure.PreferenceKey, 1.0f);
            _session.Setting.ShowAbsolutePressure.Value = PreferenceManager.GetIfExists(_session.Setting.ShowAbsolutePressure.PreferenceKey, false);
*/
            foreach (Component c in _session.Mapping.Components())
            {
                c.Enabled = PreferenceManager.GetSensorSettings(c.BoardID).Enabled;

                if (c is IWarningLimits sc)
                {
                    sc.MinLimit = PreferenceManager.GetSensorSettings(c.BoardID).Min;
                    sc.MaxLimit = PreferenceManager.GetSensorSettings(c.BoardID).Max;
                }
            }
        }

        public void UpdateSession(Preferences updated)
        {   
            // Update settings
            PreferenceManager.UpdatePreferences(updated);
            
            // Update Old Component with new settings
            foreach (Component c in _session.Mapping.Components())
            {
                SensorSettings settings;
                if (PreferenceManager.Manager.Preferences.Visual.SensorVisuals.TryGetValue(c.BoardID, out settings))
                {
                    c.Enabled = settings.Enabled;

                    if (c is IWarningLimits w)
                    {
                        w.MinLimit = settings.Min;
                        w.MaxLimit = settings.Max;
                    }    
                }
            }
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
