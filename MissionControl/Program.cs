using System;
using Gtk;
using MissionControl.Connection;
using MissionControl.Data;
using MissionControl.UI;

namespace MissionControl
{
    class Program : IUIEvents
    {
        public static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program() 
        {
            LogThread logThread = new LogThread();
            IOThread ioThread = new IOThread();
            UserInterface ui = new UserInterface();
            // When UserInterface loops stop so should other threads
            logThread.Stop();
            ioThread.Stop();
        }


        public void onStatePressed()
        {
            throw new NotImplementedException();
        }

        public void OnValvePressed()
        {
            throw new NotImplementedException();
        }
    }
}
