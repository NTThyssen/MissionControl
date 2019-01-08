using System;
using Gtk;

namespace MissionControl.UI
{
    public partial class TestStandView : Window
    {
        public TestStandView() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            DeleteEvent += OnDeleteEvent;
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}
