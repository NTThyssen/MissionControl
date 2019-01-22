using System;
using Gtk;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
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

        public Program(IDataStore dataStore, ILogThread logThread, IIOThread ioThread, IUserInterface ui) 
        {
            _dataStore = dataStore;
            _logThread = logThread;
            _ioThread = ioThread;
            _ui = ui;
            //_logThread.StartThread();
            _ioThread.StartThread();

            // Blocking call
            _ui.StartUI(this);

            // When UserInterface loops stop so should other threads
            _logThread.StopLogging();
            _ioThread.StopThread();
        }

        public static void Main(string[] args)
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);

            LogThread logThread = new LogThread(dataStore);

            SerialPort port = new SerialPort();
            IOThread ioThread = new IOThread(dataStore, ref session, port);

            UserInterface ui = new UserInterface(ref session);
            Program p = new Program(dataStore, logThread, ioThread, ui);
        }


        public void OnStatePressed(StateCommand command)
        {
            State state = _dataStore.GetCurrentSession().Mapping.States().Find((State obj) => obj.StateID == command.CommandValue());
            _dataStore.GetCurrentSession().State = state;
        }

        public void OnValvePressed()
        {
            throw new NotImplementedException();
        }

        public void OnSessionSettingsUpdated(Session session)
        {
            _dataStore.UpdateSession(session);
            PreferenceManager.Preferences[PreferenceManager.STD_LOGFOLDER] = session.LogFilePath;
            PreferenceManager.Preferences[PreferenceManager.STD_PORTNAME] = session.PortName;
            PreferenceManager.Preferences.Save();
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
    }
}
