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
        void OnMenuSettingsPressed();
        void OnMenuPlotViewerPressed();
        void OnStatePressed(StateCommand state);
        void OnServoPressed(ServoComponent servo, float value);
        void OnSolenoidPressed(SolenoidComponent solenoid, bool open);
        void OnLogStartPressed();
        void OnLogStopPressed();
        void OnEmergencyCombinationPressed();
    }

    public partial class TestStandView : Window, ILockable, IValveControlListener, IStateControlListener
    {

        SVGView _svgWidget;
        ValveControlWidget _valveWidget;
        StateControlWidget _stateWidget;
        ITestStandViewListener _listener;
        Session _session;

        Button _btnLogStart, _btnLogStop;

        Label _lastConnection;
        Gdk.Color _clrGoodConnection = new Gdk.Color(0, 255, 0);
        Gdk.Color _clrBadConnection = new Gdk.Color(255, 0, 0);

        bool _spaceDown, _escDown;

        public TestStandView(ITestStandViewListener listener, ref Session session) :
                base(Gtk.WindowType.Toplevel)
        {
            Title = "Control Panel";
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);

            _listener = listener;
            _session = session;

            Layout();
            DeleteEvent += OnDeleteEvent;
            KeyPressEvent += WindowKeyPress;
            KeyReleaseEvent += WindowKeyRelease;
            ShowAll();
                
        }

        public void Layout()
        {
           this.Build();

            // Background color
            ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            _svgWidget = new SVGView(@"Resources/TestStand.svg", ref _session);
            _valveWidget = new ValveControlWidget(_session.Mapping.Components(), this);
            _stateWidget = new StateControlWidget(_session.Mapping.States(), this);
            _stateWidget.SetCurrentState(_session.State);

            VBox controlPane1 = new VBox(false, 8);

            VBox logButtonContainer = new VBox(false, 8);
            DSectionTitle logTitle = new DSectionTitle("Logging");
            HBox logButtons= new HBox(true, 8);
            _btnLogStart = new Button
            {
                Label = "Start log",
                HeightRequest = 40
            };
            _btnLogStart.Pressed += LogStartPressed;
            _btnLogStart.ModifyBg(StateType.Insensitive, new Gdk.Color(70, 70, 70));

            _btnLogStop = new Button
            {
                Label = "Stop log",
                HeightRequest = 40
            };
            _btnLogStop.Pressed += LogStopPressed;
            _btnLogStop.Sensitive = false;
            _btnLogStop.ModifyBg(StateType.Insensitive, new Gdk.Color(70, 70, 70));

            logButtons.PackStart(_btnLogStart, false, true, 0);
            logButtons.PackStart(_btnLogStop, false, true, 0);

            _lastConnection = new Label { Text = "\n\n" };
            _lastConnection.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
            _lastConnection.SetAlignment(0, 0.5f);

            logButtonContainer.PackStart(logTitle, false, false, 0);
            logButtonContainer.PackStart(logButtons, false, false, 0);
            logButtonContainer.PackStart(_lastConnection, false, false, 0);

            controlPane1.PackStart(_valveWidget, false, false, 0);
            controlPane1.PackStart(logButtonContainer, false, false, 20);

            // Horizonal layout
            HBox horizontalLayout = new HBox(false, 0);
            horizontalLayout.PackStart(_svgWidget, true, true, 0);
            horizontalLayout.PackStart(controlPane1, false, false, 20);
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
            settingsItem.Activated += (sender, e) => _listener.OnMenuSettingsPressed();
            menu.Append(settingsItem);

            MenuItem plotItem = new MenuItem("Plot Viewer");
            plotItem.Activated += (sender, e) => _listener.OnMenuPlotViewerPressed();
            menu.Append(plotItem);

            verticalLayout.PackStart(menu, false, false, 0);
            verticalLayout.PackStart(align, true, true, 0);

            Add(verticalLayout);

            SetPosition(WindowPosition.Center);

            ShowAll();
        }

        void LogStartPressed(object sender, EventArgs e)
        {
            _btnLogStart.Sensitive = false;
            _btnLogStop.Sensitive = true;
            _listener.OnLogStartPressed();
        }

        void LogStopPressed(object sender, EventArgs e)
        {
            _btnLogStart.Sensitive = true;
            _btnLogStop.Sensitive = false;
            _listener.OnLogStopPressed();
        }


        public void Update() {
            _svgWidget.Refresh();
            _stateWidget.SetCurrentState(_session.State);
            UpdateLastConnectionLabel();
        }

        public void UpdateLastConnectionLabel ()
        {
            double time = (DateTime.Now - _session.LastReceived).Milliseconds / 1000.0;
            _lastConnection.Text = String.Format("Time since package:\n{0} s", time);
            _lastConnection.ModifyFg(StateType.Normal, (time < 4) ? _clrGoodConnection : _clrBadConnection);
        }

        [GLib.ConnectBefore]
        protected void WindowKeyPress(object sender, KeyPressEventArgs args)
        {
            _spaceDown |= args.Event.Key == Gdk.Key.space;
            _escDown |= args.Event.Key == Gdk.Key.Escape;

            if (_spaceDown && _escDown)
            {
                _listener.OnEmergencyCombinationPressed();
                Update();
            }
        }

        void WindowKeyRelease(object o, KeyReleaseEventArgs args)
        {
            _spaceDown &= args.Event.Key == Gdk.Key.space;
            _escDown &= args.Event.Key == Gdk.Key.Escape;
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
            _listener.OnStatePressed(command);

            // Fake response
            _stateWidget.SetCurrentState(_session.State);
        }

        public void OnServoPressed(ServoComponent servo, float value)
        {
            _listener.OnServoPressed(servo, value);

        }

        public void OnSolenoidPressed(SolenoidComponent solenoid, bool open)
        {
            _listener.OnSolenoidPressed(solenoid, open);
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
