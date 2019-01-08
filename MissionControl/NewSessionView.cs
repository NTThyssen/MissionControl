using System;
namespace MissionControl
{
    public partial class NewSessionView : Gtk.Window
    {
        public NewSessionView() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
    }
}
