using System;
using System.Collections.Generic;
using System.Globalization;
using System.Timers;
using GLib;
using Gtk;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI;
using Svg;

namespace MissionControl
{
    class Program : IUIEvents, IConnectionListener
    {
        IDataStore _dataStore;
        ILogThread _logThread;
        IIOThread _ioThread;
        IUserInterface _ui;

        private static bool _isUsingSimulatedSerialPort = false;

        public Program(IDataStore dataStore, ILogThread logThread, IIOThread ioThread, IUserInterface ui) 
        {
            _dataStore = dataStore;
            _logThread = logThread;
            _ioThread = ioThread;
            _ui = ui;

            // Blocking call
            _ui.StartUI(this);

            // When UserInterface loops stop so should other threads
            _logThread.StopLogging();
            _ioThread.StopConnection();
        }

        public static void Main(string[] args)
        {
            PreferenceManager manager = PreferenceManager.Manager;
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);

            LogThread logThread = new LogThread(dataStore);
         

            _isUsingSimulatedSerialPort = args.Length > 0 && args[0].ToLower().Equals("sim");


            IOThread ioThread = new IOThread(dataStore, ref session);

            UserInterface ui = new UserInterface(ref session);
            Program p = new Program(dataStore, logThread, ioThread, ui);
        }


        public void OnCommand(Command command)
        {
            _ioThread.SendCommand(command);
        }

        public void OnTimedCommand(Command cmd1, Command cmd2, int delayMillis)
        {
            _ioThread.SendCommand(cmd1);
            Timer timer = new Timer();
            timer.Interval = delayMillis;
            timer.Elapsed += (sender, args) =>
            {
                _ioThread.SendCommand(cmd2);
                /*_ioThread.SendCommand(cmd2);
                _ioThread.SendCommand(cmd2);
                _ioThread.SendCommand(cmd2);
                _ioThread.SendCommand(cmd2);*/
                timer.Stop();
            };
            timer.Start();
        }

        public void OnSessionSettingsUpdated(Preferences preferences)
        {
            _dataStore.UpdateSession(preferences);
        }

        public void OnEmergencyState()
        {
        }

        public void OnLogStartPressed()
        {
            _dataStore.EnableLogging();
            _logThread.StartLogging();
        }

        public void OnLogStopPressed()
        {
            _dataStore.DisableLogging();
            _logThread.StopLogging();
        }

        public void OnConnectPressed()
        {

            ISerialPort port;
            Session currentSession = _dataStore.GetCurrentSession();
            if (_isUsingSimulatedSerialPort)
            {
                port = new SimSerialPort(currentSession.Mapping);
            }
            else
            {
                SerialSettings serial = PreferenceManager.Manager.Preferences.System.Serial;
                port = new SerialPort(serial.PortName, serial.BaudRate);
            }
            _ioThread.StartConnection(port, this);
        }

        public void OnDisconnectPressed()
        {
            _ioThread.StopConnection();
        }

    
        public void OnStartAutoSequencePressed()
        {
            AutoSequenceCommand command = new AutoSequenceCommand(true);
            _ioThread.SendCommand(command);
        }

        public void OnStopAutoSequencePressed()
        {
            AutoSequenceCommand command = new AutoSequenceCommand(false);
            _ioThread.SendCommand(command);

            // Fake response
            _dataStore.GetCurrentSession().IsAutoSequence = false;
        }

        public void OnFuelTankFillSet(float mass)
        {
            if (_dataStore.GetCurrentSession().Mapping.ComponentsByID()[24] is TankComponent tc)
            {
                tc.SetInputVolume(_dataStore.GetCurrentSession().SystemTime, mass);    
            }
            else
            {
                Console.Write("Cannot set fill on fuel tank");
            }
        }

        public void OnAutoParametersSet(AutoParameters ap)
        {
            PreferenceManager.Manager.Preferences.AutoSequence = ap;
            PreferenceManager.Manager.Save();
            
            AutoParametersCommand command = new AutoParametersCommand(ap, _dataStore.GetCurrentSession().Mapping);
            _ioThread.SendCommand(command);
        }

        public void OnTarePressed()
        {
            if (_dataStore.GetCurrentSession().Mapping.ComponentsByID()[0] is LoadComponent lc)
            {
                lc.Tare();
             }
        }

        public void OnAcknowledge(Acknowledgement acknowledgement)
        {
            switch (acknowledgement.Command)
            {
                case AutoSequenceCommand asc:
                    if (asc.Auto && !_dataStore.GetCurrentSession().IsAutoSequence)
                    {
                        _dataStore.GetCurrentSession().IsAutoSequence = true;
                        _dataStore.GetCurrentSession().AutoSequenceStartTime = _dataStore.GetCurrentSession().SystemTime;
                    }
                    else if (!asc.Auto && _dataStore.GetCurrentSession().IsAutoSequence)
                    {
                        _dataStore.GetCurrentSession().IsAutoSequence = false;
                    }
                    break;
            } 
        }
    }
}
