using System;
using Gtk;
using MissionControl.Connection;
using MissionControl.Data;
using MissionControl.UI;
using Svg;

namespace MissionControl
{
    class Program : IUIEvents
    {
    
        IIOThread _ioThread;
        IUserInterface _ui;

        public Program(IDataStore dataStore, IIOThread ioThread, IUserInterface ui) 
        {
            _ioThread = ioThread;
            _ui = ui;
            //_logThread.StartThread();
            //_ioThread.StartThread();

            // Blocking call
            _ui.StartUI(new TestStandMapping());

            // When UserInterface loops stop so should other threads
            //_logThread.StopThread();
            _ioThread.StopThread();
        }


        public void onStatePressed()
        {
            throw new NotImplementedException();
        }

        public void OnValvePressed()
        {
            throw new NotImplementedException();
        }

        public static void Main(string[] args)
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(null, mapping);
            DataStore dataStore = new DataStore(session);
            IOThread ioThread = new IOThread(dataStore, null);
            UserInterface ui = new UserInterface();
            Program p = new Program(dataStore, ioThread, ui);
        }
    }
}
