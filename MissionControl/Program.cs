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

        ILogThread _logThread;
        IIOThread _ioThread;
        IUserInterface _ui;

        public Program(ILogThread logThread, IIOThread ioThread, IUserInterface ui) 
        {
            _logThread =  logThread;
            _ioThread = ioThread;
            _ui = ui;
            //_logThread.StartThread();
            //_ioThread.StartThread();

            // Blocking call
            _ui.StartUI(new TestStandMapping());

            // When UserInterface loops stop so should other threads
            _logThread.StopThread();
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
            Program p = new Program(new LogThread(), new IOThread(), new UserInterface());
        }
    }
}
