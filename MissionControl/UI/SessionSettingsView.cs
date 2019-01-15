using System;
using Gtk;

namespace MissionControl.UI
{
    public interface ISessionSettingsViewListener
    {
    }

    public partial class SessionSettingsView : Window
    {

        ISessionSettingsViewListener _eventHandler;
        public SessionSettingsView(ISessionSettingsViewListener handler) : base(WindowType.Toplevel)
        {
            _eventHandler = handler;
            Build();
           
        }

        protected void OnButton1Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Button clicked");

        }
    }
}