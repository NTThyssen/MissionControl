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
        AutoRunConfig _autoRunConfig;

        private IUIEvents _listener;
        Session _session;

        private const uint UPDATE_FREQ_FOREGROUND = 500;
        private const uint UPDATE_FREQ_BACKGROUND = 2500;
        uint _updateSVGTimer;

        public UserInterface(ref Session session)
        {
            _session = session;

        }

        public void StartUI(IUIEvents listener)
        {
            Application.Init();

            _listener = listener;
            if (_session.Setting.ShowSettingsOnStartup.Value)
            {
                ShowSessionSettings(true);
            }
            else
            {
                ShowTestStandView();
            }

            Application.Run();
        }

        public bool UpdateControls() {
            if(_testStandView != null)
            {
                _testStandView.UpdateControls();
            }
            return true;
        }

        public bool UpdateSVG()
        {
            if (_testStandView != null)
            {
                _testStandView.UpdateSVG();
            }
            return true;
        }

        public void ShowSessionSettings(bool initialWindow)
        {
            if (_newSessionView == null)
            {
                _newSessionView = new SessionSettingsView(this, _session);
            }
            else
            {
                _newSessionView.Destroy();
                _newSessionView = new SessionSettingsView(this, _session);
            }

            if (initialWindow)
            {
                _newSessionView.DeleteEvent += (o, args) => Application.Quit();
            }
        }

        public void ShowTestStandView()
        {
            _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_FOREGROUND);
            GLib.Timeout.Add(200, new GLib.TimeoutHandler(UpdateControls));
            _testStandView = new TestStandView(this, ref _session);
        }

        public void ShowPlotViewer()
        {
            if (_plotView == null)
            {
                _plotView = new PlotView();
            }
            else
            {
                _plotView.Destroy();
                _plotView = new PlotView();
            }
            _plotView.DeleteEvent += (object o, DeleteEventArgs args) => _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_FOREGROUND);
            _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_BACKGROUND);
        }
        
        public void ShowAutoConfig()
        {
            if (_plotView == null)
            {
                _autoRunConfig = new AutoRunConfig();
            }
            else
            {
                _autoRunConfig.Destroy();
                _autoRunConfig = new AutoRunConfig();
            }
            _autoRunConfig.DeleteEvent += (object o, DeleteEventArgs args) => _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_FOREGROUND);
            _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_BACKGROUND);
        }

        public void OnMenuSettingsPressed()
        {
            ShowSessionSettings(false);
        }

        public void OnMenuPlotViewerPressed()
        {
            ShowPlotViewer();
        }

       

        public void OnMenuAutoRunConfigPressed()
        {
            ShowAutoConfig();
            
        }


        public void OnStatePressed(State state)
        {
            StateCommand command = new StateCommand(state.StateID);
            _listener.OnCommand(command);
        }

        public void OnServoPressed(ServoComponent servo, float value)
        {
            ServoCommand command = new ServoCommand(servo.BoardID, value);
            _listener.OnCommand(command);
        }

        public void OnSolenoidPressed(SolenoidComponent solenoid, bool open)
        {
            SolenoidCommand command = new SolenoidCommand(solenoid.BoardID, open);
            _listener.OnCommand(command);
        }

        public void OnSave(Session session)
        {
            if (_listener != null)
            {
                _listener.OnSessionSettingsUpdated(session);
            }

            if (_newSessionView != null) _newSessionView.Destroy();

            if (_testStandView == null)
            {
                ShowTestStandView();
            }
            else
            {
                _testStandView.UpdateSVG();
                _testStandView.UpdateControls();
            }
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

        public uint SetUpdateSVGFrequency(uint oldTimer, uint ms)
        {
            GLib.Source.Remove(oldTimer);
            return GLib.Timeout.Add(ms, new GLib.TimeoutHandler(UpdateSVG));
        }

        public void OnConnectPressed()
        {
            _listener.OnConnectPressed();
        }

        public void OnDisconnectPressed()
        {
            _listener.OnDisconnectPressed();
        }

        public void OnStartAutoSequencePressed()
        {
            _listener.OnStartAutoSequencePressed();
        }
        
        public void OnStopAutoSequencePressed()
        {
            _listener.OnStopAutoSequencePressed();
        }
    }
}
