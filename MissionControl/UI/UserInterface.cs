using System;
using Gtk;
using MissionControl.Data;

namespace MissionControl.UI
{
    public interface IUserInterface
    {
        void ShowTestStandView(TestStandMapping map);
        void ShowSessionSettings();
        void ShowPlotViewer();
        void StartUI(TestStandMapping map);
    }

    public class UserInterface : IUserInterface, ITestStandViewListener, ISessionSettingsViewListener
    {

        SessionSettingsView _newSessionView;
        TestStandView _testStandView;
        PlotView _plotView;

        public UserInterface()
        {

        }

        public void StartUI(TestStandMapping map) {
            Application.Init();
            //ShowNewSessionView();
            ShowTestStandView(map);
            //ShowPlotView();
            Application.Run();
        }

        public void ShowSessionSettings()
        {
            _newSessionView = new SessionSettingsView(this);
        }

        public void ShowTestStandView(TestStandMapping map)
        {
            if (_newSessionView != null) _newSessionView.Destroy();
            _testStandView = new TestStandView(this, map);
        }

        public void ShowPlotViewer()
        {
            _plotView = new PlotView();
        }

        public void MenuSettingsPressed()
        {
            ShowSessionSettings();
        }

        public void MenuPlotViewerPressed()
        {
            ShowPlotViewer();
        }
    }
}
