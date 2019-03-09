using System;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.UI
{
    public interface IUIEvents
    {
        void OnCommand(Command command);
        void OnTimedCommand(Command cmd1, Command cmd2, int delayMillis);
        void OnSessionSettingsUpdated(Preferences preferences);
        void OnEmergencyState();
        void OnLogStartPressed();
        void OnLogStopPressed();
        void OnConnectPressed();
        void OnDisconnectPressed();
        void OnStartAutoSequencePressed();
        void OnStopAutoSequencePressed();
        void OnFuelTankFillSet(float mass);
        void OnAutoParametersSet(AutoParameters ap);
        void OnTarePressed();

    }
}
