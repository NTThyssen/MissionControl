using System;
using System.Threading;
using MissionControl.Connection.Commands;

namespace MissionControl.Connection
{
    public class IOThread : IIOThread
    {
        Thread t;

        public IOThread()
        {
            t = new Thread(runMethod);
            t.Name = "IO Thread";
        }


        public void StartThread() { t.Start(); }
        public void StopThread() { t.Abort(); }

        public void SendCommand(Command cmd)
        {
         
        }

        private void runMethod()
        {

            while (true)
            {
                Console.WriteLine("Thread: {0}", Thread.CurrentThread.Name);
                Thread.Sleep(3000);
            }
        }
    }
}
