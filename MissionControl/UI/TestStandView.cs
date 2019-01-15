using System;
using Gtk;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI.Widgets;

namespace MissionControl.UI
{

    public interface ITestStandViewListener
    {
        void MenuSettingsPressed();
        void MenuPlotViewerPressed();
    }

    public partial class TestStandView : Window, ILockable, IValveControlListener, IStateControlListener
    {

        SVGWidget _svgWidget;
        ValveControlWidget _valveWidget;
        StateControlWidget _stateWidget;
        ITestStandViewListener _listener;

        public TestStandView(ITestStandViewListener listener, TestStandMapping map) :
                base(Gtk.WindowType.Toplevel)
        {
            Title = "Control Panel";
            _listener = listener;
            _svgWidget = new SVGWidget(@"Resources/TestStand.svg", map);
            _valveWidget = new ValveControlWidget(map.Components(), this);
            _stateWidget = new StateControlWidget(map.States(), this);
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
            HBox horizontalLayout = new HBox(false, 0);
            horizontalLayout.PackStart(_svgWidget, true, true, 0);
            horizontalLayout.PackStart(_valveWidget, false, false, 20);
            horizontalLayout.PackStart(_stateWidget, false, false, 20);

            // Window layout
            Alignment align = new Alignment(0.5f, 0.5f, 1, 1)
            {
                TopPadding = 20,
                LeftPadding = 20,
                RightPadding = 20,
                BottomPadding = 20
            };
            align.Add(horizontalLayout);

            VBox verticalLayout = new VBox(false, 0);
  
            MenuBar menu = new MenuBar();

            MenuItem settingsItem = new MenuItem("Settings");
            settingsItem.Activated += (sender, e) => _listener.MenuSettingsPressed();
            menu.Append(settingsItem);

            MenuItem plotItem = new MenuItem("Plot Viewer");
            plotItem.Activated += (sender, e) => _listener.MenuPlotViewerPressed();
            menu.Append(plotItem);

            verticalLayout.PackStart(menu, false, false, 0);
            verticalLayout.PackStart(align, true, true, 0);

            Add(verticalLayout);

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

        public void OnStatePressed(StateCommand command)
        {
            Console.WriteLine("State pressed: {0}", command.StateName);
            _stateWidget.SetCurrentState(command);
        }

        public void OnServoPressed(ServoComponent servo, float value)
        {
            Console.WriteLine("{0}: {1}", servo.Name, value);
        }

        public void OnSolenoidPressed(SolenoidComponent solenoid, bool active)
        {
            Console.WriteLine("{0}: {1}", solenoid.Name, active);
        }

        public void OnControlEnter(ValveComponent component)
        {
            _svgWidget.MarkValve(component);
        }

        public void OnControlLeave(ValveComponent component)
        {
            _svgWidget.UnmarkValve(component);
        }
    }
}
