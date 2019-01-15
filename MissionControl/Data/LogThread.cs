using System;
using System.Threading;

namespace MissionControl.Data
{
    public interface ILogThread
    {
        void StartLogging(string filepath);
        void StopLogging();
        void StartThread();
        void StopThread();
    }

    public class LogThread : ILogThread
    {
        Thread t;
        string _filepath;
        IDataLog _dataLog;

        public LogThread(IDataLog dataLog)
        {
            t = new Thread(RunMethod);
            t.Name = "Logger Thread";
            _dataLog = dataLog;
        }

        public void StartThread() { t.Start(); }
        public void StopThread() { t.Abort(); }

        public void StartLogging(string filepath) {
            _filepath = filepath;
        }
        public void StopLogging() { }

        private void RunMethod() {
        
            while(true) {

                while (!_dataLog.Empty())
                {
                    DataPacket packet = _dataLog.Dequeue();
                    // Write to file
                }

                Console.WriteLine("Thread {0}", Thread.CurrentThread.Name);
                Thread.Sleep(3000);
            }
        }
    }
}
