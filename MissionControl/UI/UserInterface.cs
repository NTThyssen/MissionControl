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

    public class UserInterface : IUserInterface, ITestStandViewListener, ISessionSettingsViewListener, IAutoParameterListener
    {

        SessionSettingsView _newSessionView;
        TestStandView _testStandView;
        PlotView _plotView;
        AutoRunConfigView _autoRunConfigView;
        LoadComponent loadcell;

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
            if (PreferenceManager.Manager.Preferences.Visual.ShowSettingsOnStartup)
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
            if (_autoRunConfigView == null)
            {
                _autoRunConfigView = new AutoRunConfigView(this, PreferenceManager.Manager.Preferences.AutoSequence);
            }
            else
            {
                _autoRunConfigView.Destroy();
                _autoRunConfigView = new AutoRunConfigView(this, PreferenceManager.Manager.Preferences.AutoSequence);
            }
            _autoRunConfigView.DeleteEvent += (object o, DeleteEventArgs args) => _updateSVGTimer = SetUpdateSVGFrequency(_updateSVGTimer, UPDATE_FREQ_FOREGROUND);
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

        public void OnFuelTankFillSet(float mass)
        {
            _listener.OnFuelTankFillSet(mass);
        }

        public void OnTarePressed()
        {
            _listener.OnTarePressed();
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

        public void OnSave(Preferences preferences)
        {
            if (_listener != null)
            {
                _listener.OnSessionSettingsUpdated(preferences);
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

        public void OnParametersSave(AutoParameters ap)
        {
            _listener.OnAutoParametersSet(ap);
            _autoRunConfigView?.Destroy();
        }
    }
}
