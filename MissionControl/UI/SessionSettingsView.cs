using System;
using Gtk;

namespace MissionControl.UI
{
    public partial class SessionSettingsView : Window
    {

        ISessionSettingsViewListener _eventHandler;
        public SessionSettingsView(ISessionSettingsViewListener handler) : base(WindowType.Toplevel)
        {
            _eventHandler = handler;
            Build();
            DeleteEvent += OnDeleteEvent;
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        protected void OnButton1Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Button clicked");

        }
    }
}