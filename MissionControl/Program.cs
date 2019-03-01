using System;
using System.Globalization;
using Gtk;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI;
using Svg;

namespace MissionControl
{
    class Program : IUIEvents
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

            if (command is StateCommand sc)
            {
                // Fake response
                State state = _dataStore.GetCurrentSession().Mapping.States().Find((State obj) => obj.StateID == sc.StateID);
                if (state != null) _dataStore.GetCurrentSession().State = state;
            }

        }

        public void OnValvePressed(ValveCommand command)
        {
            _ioThread.SendCommand(command);
        }

        public void OnSessionSettingsUpdated(Session session)
        {
            _dataStore.UpdateSession(session);
        }

        public void OnEmergencyState()
        {
            StateCommand command = new StateCommand(_dataStore.GetCurrentSession().Mapping.EmergencyState.StateID);
            _ioThread.SendEmergency(command);
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
            if (_isUsingSimulatedSerialPort)
            {
                port = new SimSerialPort(_dataStore.GetCurrentSession().Mapping);
            }
            else
            {
                port = new SerialPort();
            }
            _ioThread.StartConnection(port);
        }

        public void OnDisconnectPressed()
        {
            _ioThread.StopConnection();
        }

        public void OnStartAutoSequencePressed()
        {

            AutoSequenceCommand command = new AutoSequenceCommand(true);
            _ioThread.SendCommand(command);

            // Fake response
            _dataStore.GetCurrentSession().IsAutoSequence = true;
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
            AutoParametersCommand command = new AutoParametersCommand(ap);
            _ioThread.SendCommand(command);
        }
    }
}
