using System;
using Gtk;
using MissionControl.Data;

namespace MissionControl.UI
{
    public partial class TestStandView : Window, ILockable
    {

        SVGWidget _svgWidget;
        ITestStandViewListener _listener;

        public TestStandView(ITestStandViewListener listener, TestStandMapping map) :
                base(Gtk.WindowType.Toplevel)
        {
            _listener = listener;
            _svgWidget = new SVGWidget(@"Resources/TestStand.svg", map);
            Layout();
            DeleteEvent += OnDeleteEvent;
            KeyPressEvent += WindowKeyPress;
        }

        public void Layout()
        {
            this.Build();

            // Background color
            ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            // Horizonal layout
            HBox horizontalLayout = new HBox();
            horizontalLayout.PackStart(_svgWidget);

            // Window layout
            Add(horizontalLayout);

            SetPosition(WindowPosition.Center);

            ShowAll();
        }

        [GLib.ConnectBefore]
        protected void WindowKeyPress(object sender, KeyPressEventArgs args)
        {
            Console.WriteLine("{0} was pressed!", args.Event.Key.ToString());
            if (args.Event.Key == Gdk.Key.space)
            {
                _svgWidget.Refresh();
            }
        }
    

        public void Lock()
        {
            throw new NotImplementedException();
        }

        public void Unlock()
        {
            throw new NotImplementedException();
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            KeyPressEvent -= WindowKeyPress;
            Application.Quit();
            a.RetVal = true;
        }
    }
}
