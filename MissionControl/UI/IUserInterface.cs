using System;
using MissionControl.Data;

namespace MissionControl.UI
{
    public interface IUserInterface
    {
        void ShowTestStandView(TestStandMapping map);
        void ShowNewSessionView();
        void StartUI(TestStandMapping map);
    }
}
