using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
using System.Linq;
using System.IO;

namespace MissionControl.Connection
{

    public interface IIOThread
    {
        void SendCommand(Command cmd);
        void SendEmergency(Command cmd);
        void StartConnection(ISerialPort port);
        void StopConnection();
    }

    public class IOThread : IIOThread
    {
        Thread t;
        IDataLog _dataLog;
        Queue<Command> _commands;
        Session _session;
        ISerialPort _port;
        bool _shouldRun;

        public List<Command> Commands => _commands.ToList();

        public IOThread(IDataLog dataLog, ref Session session)
        {
            _dataLog = dataLog;
            _commands = new Queue<Command>();
            _session = session;
        }

        public void StartConnection(ISerialPort port) {
            if (t != null && t.ThreadState == ThreadState.Running)
            {
                StopConnection();
            }

            _port = port;
            t = new Thread(RunMethod) { Name = "IO Thread" };
            _shouldRun = true;
            t.Start(); 
        }

        public void StopConnection() {
            _shouldRun = false;
            if (t != null && t.ThreadState == ThreadState.Running)
            {
                t.Join(2000);
            }
            _session.Connected = false;
         }

        public void SendCommand(Command cmd)
        {
            Console.WriteLine("Command: \"{0}\" queued!", cmd);
            _commands.Enqueue(cmd);
        }

        public void SendEmergency(Command cmd)
        {
            Console.WriteLine("Sending emergency!");
            _commands.Clear();
            _commands.Enqueue(cmd);
        }

        List<byte> buffered;
        bool reading;

        const int bufsize = 8;
        int highs, lows;
        
        byte[] startFence = {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
        byte[] endFence = {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};

        private Queue<byte> _fenceQueue = new Queue<byte>();

        public void AddToFenceQueue(byte b, int size)
        {
            _fenceQueue.Enqueue(b);
            int dif = Math.Max(_fenceQueue.Count - size, 0);
            for (int i = 0; i < dif; i++)
            {
                _fenceQueue.Dequeue();
            }
        }
        
        public bool IsFence(Queue<byte> actual, byte[] expected)
        {
            byte[] actualBytes = actual.ToArray();
            
            if (actualBytes.Length != expected.Length)
            {
                return false;
            }

            for (int i = 0; i < actualBytes.Length; i++)
            {
                if (actualBytes[i] != expected[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        //byte[] fakeBuffer = { 0xA, 0xFF, 0xC, 0xFF, 0x01, 0x14, 0x0B, 0xD0, 0xC8, 0x02, 0xC9, 0x00, 0x00, 0x27, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xA };

        public void RunMethod()
        {
            buffered = new List<byte>();
            _commands.Clear();
            Open();

            while (_shouldRun && _port.IsOpen)
            {
                WriteAll();
                ReadAll();
            }

            _session.Connected = false;
            try
            {
                _port.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Serial IO error: {0}", e.Message);
            }
        }

        private void Open()
        {
            try
            {
                _port.Open();
                _port.DiscardInBuffer();
                _session.Connected = true;
                return;
            }
            catch (IOException e)
            {
                Console.WriteLine("Serial IO error: {0}", e.Message);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Serial invalid operation error: {0}", e.Message);
            }
            StopConnection();
        }

        private void WriteAll()
        {
            while (_commands.Count > 0)
            {
                
                byte[] byteData = _commands.Dequeue().ToByteData();
                byte[] wbuffer = new byte[startFence.Length + endFence.Length + byteData.Length];
                Array.Copy(startFence, 0, wbuffer, 0, startFence.Length);
                Array.Copy(byteData, 0, wbuffer, startFence.Length, byteData.Length);
                Array.Copy(endFence, 0, wbuffer, startFence.Length + byteData.Length, endFence.Length);
                
                try
                {
                    _port.Write(wbuffer, 0, wbuffer.Length);   
                    Console.Write("Writing: ");
                    foreach (byte b in wbuffer)
                    {
                        Console.Write("{0:X} ", b);
                    }
                    Console.WriteLine();
                } catch (IOException)
                {
                    Console.WriteLine("While writing an IOException occured");
                    StopConnection();
                    return;
                }

            }
        }

        private void ReadAll()
        {
            byte[] buf = new byte[0];
            int bytesRead = 0;
            try
            {
                if (_port.BytesToRead > 0 && _shouldRun)
                {
                    int bytes = Math.Min(_port.BytesToRead, 8);
                    buf = new byte[bytes];
                    bytesRead = _port.Read(buf, 0, bytes);
                }
            } catch (IOException)
            {
                Console.WriteLine("While reading an IOException occured");
                StopConnection();
                return;
            }

            //Console.WriteLine("{0} bytes read", bytesRead);
            for (int i = 0; i < bytesRead; i++)
            {
                byte b = buf[i];
                // Search for starts and ends
                
                if (reading)
                {
                    AddToFenceQueue(b, endFence.Length);
                    if (IsFence(_fenceQueue, endFence))
                    {
                        reading = false;
                        PackageDone();
                    }
                    else
                    {
                        buffered.Add(b);    
                    }
                }
                else
                {
                    AddToFenceQueue(b, startFence.Length);
                    reading = IsFence(_fenceQueue, startFence); 
                }
                
            }
        }

        private void PackageDone()
        {
            foreach (byte b in buffered)
            {
                Console.Write("{0:X} ", b);
            }
            Console.WriteLine();

            DataPacket packet = new DataPacket(buffered.ToArray());
            _dataLog.Enqueue(packet);

            buffered.Clear();
        }


    }
}
