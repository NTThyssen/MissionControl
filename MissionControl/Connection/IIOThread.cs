using MissionControl.Connection.Commands;

namespace MissionControl.Connection
{
    public interface IIOThread
    {
        void SendCommand(Command cmd);
        void StartThread();
        void StopThread();
    }
}