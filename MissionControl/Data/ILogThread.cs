namespace MissionControl.Data
{
    public interface ILogThread
    {
        void StartLogging(string filepath);
        void StartThread();
        void StopLogging();
        void StopThread();
    }
}