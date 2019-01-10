using System;
using System.Threading;

namespace MissionControl.Data
{
    public class LogThread : ILogThread
    {
        Thread t;

        public LogThread()
        {
            t = new Thread(runMethod);
            t.Name = "Logger Thread";
        }


        public void StartThread() { t.Start(); }
        public void StopThread() { t.Abort(); }

        public void StartLogging(string filepath) { }
        public void StopLogging() { }

        private void runMethod() {
        
            while(true) {
                Console.WriteLine("Thread {0}", Thread.CurrentThread.Name);
                Thread.Sleep(3000);
            }
        }
    }
}
