using System;
using MissionControl.Connection.Commands;
using MissionControl.Data;

namespace MissionControl.UI
{
    public interface IUIEvents
    {
        void OnValvePressed();
        void OnStatePressed(StateCommand state);
        void OnSessionSettingsUpdated(Session session);
        void OnEmergencyState();
        void OnLogStartPressed();
        void OnLogStopPressed();
    }
}
