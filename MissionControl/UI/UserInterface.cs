using System;
using Gtk;

namespace MissionControl.UI
{
    public class UserInterface : IUserInterface
    {

        NewSessionView _newSessionView;
        TestStandView _testStandView;

        public UserInterface()
        {
            Application.Init();
            ShowNewSessionView();
            Application.Run();
        }

        public void ShowNewSessionView()
        {
            _newSessionView = new NewSessionView(this);
            _newSessionView.Show();
        }

        public void ShowTestStandView()
        {
            _newSessionView.Destroy();
            _testStandView = new TestStandView();
            _testStandView.Show();
        }
    }
}
