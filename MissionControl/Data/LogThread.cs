using System;
using System.Threading;

namespace MissionControl.Data
{
    public class LogThread
    {
        Thread t;

        public LogThread()
        {
            t = new Thread(RunMethod);
            t.Name = "Logger Thread";
            Start();
        }


        public void Start() { t.Start(); }
        public void Stop() { t.Abort(); }

        public void RunMethod() {
        
            while(true) {
                Console.WriteLine("Thread {0}", Thread.CurrentThread.Name);
                Thread.Sleep(3000);
            }
        }
    }
}
