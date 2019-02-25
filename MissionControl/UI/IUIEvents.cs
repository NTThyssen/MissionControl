using System;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.UI
{
    public interface IUIEvents
    {
        void OnCommand(Command command);
        void OnSessionSettingsUpdated(Session session);
        void OnEmergencyState();
        void OnLogStartPressed();
        void OnLogStopPressed();
        void OnConnectPressed();
        void OnDisconnectPressed();
        void OnStartAutoSequencePressed();
        void OnStopAutoSequencePressed();
    }
}
