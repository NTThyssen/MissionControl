using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using MissionControl.Connection.Commands;
using MissionControl.Data;

namespace MissionControl.Connection
{

    public interface IIOThread
    {
        void SendCommand(Command cmd);
        void SendEmergency(Command cmd);
        void StartThread();
        void StopThread();
    }

    public class IOThread : IIOThread
    {
        Thread t;
        IDataLog _dataLog;
        Queue<Command> _commands;

        string PortName { get; set; }

        public IOThread(IDataLog dataLog, string portname)
        {
            t = new Thread(runMethod);
            t.Name = "IO Thread";
            _dataLog = dataLog;
            _commands = new Queue<Command>();
            PortName = portname;
        }


        public void StartThread() { t.Start(); }
        public void StopThread() { t.Abort(); }

        public void SendCommand(Command cmd)
        {
            _commands.Enqueue(cmd);
        }

        public void SendEmergency(Command cmd)
        {
            _commands.Clear();
            _commands.Enqueue(cmd);
        }

        private void runMethod()
        {

            SerialPort port = new SerialPort(PortName);
            port.BaudRate = 9600;
            try
            {
                port.Open();
                port.DiscardInBuffer();
            }
            catch (System.IO.IOException e)
            {

            }
            catch (System.InvalidOperationException e)
            {

            }

            while (port.IsOpen)
            {

                // Write
                if (_commands.Count > 0)
                {
                    int cmd = _commands.Dequeue().CommandValue();
                    port.WriteLine(""+cmd); // Quick fix, not good...
                }

                // Read

                if (port.BytesToRead > 0)
                {
                    byte[] buffer = new byte[2]; // Quick fix,
                    string msg = port.ReadLine();
                    DataPacket packet = new DataPacket(0, buffer);
                }
            }

        }
    }
}
