using System;
using Gtk;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.UI
{
    public interface IUserInterface
    {
        void ShowTestStandView();
        void ShowSessionSettings(bool initialWindow);
        void ShowPlotViewer();
        void StartUI(IUIEvents listener);
        }

    public class UserInterface : IUserInterface, ITestStandViewListener, ISessionSettingsViewListener
    {

        SessionSettingsView _newSessionView;
        TestStandView _testStandView;
        PlotView _plotView;

        private IUIEvents _listener;
        Session _session;

        public UserInterface(ref Session session)
        {
            _session = session;

        }

        public void StartUI(IUIEvents listener) {
            Application.Init();

            _listener = listener;
            ShowSessionSettings(true);
            //ShowTestStandView();
            //ShowPlotViewer();
            Application.Run();
        }

        public bool Update() {
            if(_testStandView != null)
            {
                // Should refresh SVGWidget
                _testStandView.Update();
                Console.WriteLine("Update call");
            }
            return true;
        }

        public void ShowSessionSettings(bool initialWindow)
        {
            _newSessionView = new SessionSettingsView(this, _session);
            if (initialWindow)
            {
                _newSessionView.DeleteEvent += (o, args) => Application.Quit();
            }
        }

        public void ShowTestStandView()
        {
            //GLib.Timeout.Add(500, new GLib.TimeoutHandler(Update));
            _testStandView = new TestStandView(this, ref _session);
        }

        public void ShowPlotViewer()
        {
            _plotView = new PlotView();
        }

        public void OnMenuSettingsPressed()
        {
            ShowSessionSettings(false);
        }

        public void OnMenuPlotViewerPressed()
        {
            ShowPlotViewer();
        }

        public void OnStatePressed(StateCommand state)
        {
            _listener.OnStatePressed(state);
            Console.WriteLine("State pressed: {0}", state.CommandValue());
        }

        public void OnServoPressed(ServoComponent servo, float value)
        {
            Console.WriteLine("{0}: {1}", servo.Name, value);
        }

        public void OnSolenoidPressed(SolenoidComponent solenoid, bool open)
        {
            Console.WriteLine("{0}: {1}", solenoid.Name, open);
        }

        public void OnSave(Session session)
        {
            if (_listener != null)
            {
                _listener.OnSessionSettingsUpdated(session);
            }
            if (_newSessionView != null) _newSessionView.Destroy();
            ShowTestStandView();

        }

        public void OnLogStartPressed()
        {
            if (_listener != null)
            {
                _listener.OnLogStartPressed();
            }
        }

        public void OnLogStopPressed()
        {
            if (_listener != null)
            {
                _listener.OnLogStopPressed();
            }
        }

        public void OnEmergencyCombinationPressed()
        {
            _listener.OnEmergencyState();
        }
    }
}
