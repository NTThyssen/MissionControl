using System;
using Gtk;

namespace MissionControl.UI
{
    public partial class NewSessionView : Window
    {

        IUserInterface _eventHandler;
        public NewSessionView(IUserInterface handler) : base(WindowType.Toplevel)
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
            _eventHandler.ShowTestStandView();
        }
    }
}