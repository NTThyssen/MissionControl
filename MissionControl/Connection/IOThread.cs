using System;
using System.Threading;

namespace MissionControl.Connection
{
    public class IOThread
    {
        Thread t;

        public IOThread()
        {
            t = new Thread(RunMethod);
            t.Name = "IO Thread";
            Start();
        }


        public void Start() { t.Start(); }
        public void Stop() { t.Abort(); }

        public void RunMethod()
        {

            while (true)
            {
                Console.WriteLine("Thread: {0}", Thread.CurrentThread.Name);
                Thread.Sleep(3000);
            }
        }
    }
}
