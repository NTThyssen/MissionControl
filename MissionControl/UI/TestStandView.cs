using System;
using Gtk;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI.Widgets;

namespace MissionControl.UI
{
    public partial class TestStandView : Window, ILockable
    {

        SVGWidget _svgWidget;
        ValveControlWidget _valveWidget;
        ITestStandViewListener _listener;

        public TestStandView(ITestStandViewListener listener, TestStandMapping map) :
                base(Gtk.WindowType.Toplevel)
        {
            _listener = listener;
            _svgWidget = new SVGWidget(@"Resources/TestStand.svg", map);
            _valveWidget = new ValveControlWidget(map.Components(), 
                new ServoControlWidget.ServoCallback(TestStandServoCallback), 
                new SolenoidControlWidget.SolenoidCallback(TestStandSolenoidCallback));
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
            horizontalLayout.PackStart(_svgWidget, false, false, 0);
            horizontalLayout.PackStart(_valveWidget, false, false, 20);

            // Window layout
            Alignment align = new Alignment(0.5f, 0.5f, 1, 1)
            {
                TopPadding = 20,
                LeftPadding = 20,
                RightPadding = 20,
                BottomPadding = 20
            };
            align.Add(horizontalLayout);

            Add(align);

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

        private void TestStandServoCallback(ServoComponent component, float value) { }
        private void TestStandSolenoidCallback(SolenoidComponent component, bool active) { }


        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            KeyPressEvent -= WindowKeyPress;
            Application.Quit();
            a.RetVal = true;
        }
    }
}
