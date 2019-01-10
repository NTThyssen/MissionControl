using System;
using Gtk;
using MissionControl.Data;

namespace MissionControl.UI
{
    public class UserInterface : IUserInterface, ITestStandViewListener, ISessionSettingsViewListener
    {

        SessionSettingsView _newSessionView;
        TestStandView _testStandView;

        public UserInterface()
        {

        }

        public void StartUI(TestStandMapping map) {
            Application.Init();
            //ShowNewSessionView();
            ShowTestStandView(map);
            Application.Run();
        }

        public void ShowNewSessionView()
        {
            _newSessionView = new SessionSettingsView(this);
        }

        public void ShowTestStandView(TestStandMapping map)
        {
            if (_newSessionView != null) _newSessionView.Destroy();
            _testStandView = new TestStandView(this, map);
        }
    }
}
