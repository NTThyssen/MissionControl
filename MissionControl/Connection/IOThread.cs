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
            _session.Connected = false;
            if (t != null && t.ThreadState == ThreadState.Running)
            {
                t.Join(2000);
            }
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

        const byte HIGH = 0xFF;
        const byte LOW = 0x01;
        const int endHighs = 3;
        const int endLows = 1;
        const int bufsize = 8;
        int highs, lows;

        //byte[] fakeBuffer = { 0xA, 0xFF, 0xC, 0xFF, 0x01, 0x14, 0x0B, 0xD0, 0xC8, 0x02, 0xC9, 0x00, 0x00, 0x27, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xA };

        public void RunMethod()
        {
            buffered = new List<byte>();
            Open();

            while (_shouldRun && _port.IsOpen)
            {
                WriteAll();
                ReadAll();
            }

        }

        private void Open()
        {
            try
            {
                _port.BaudRate = _session.Setting.BaudRate.Value;
                _port.PortName = _session.Setting.PortName.Value;
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
                byte[] wbuffer = _commands.Dequeue().ToByteData();
                try
                {
                    _port.Write(wbuffer, 0, wbuffer.Length);
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
                    if (lows == 1)
                    {
                        if (b == HIGH && highs == endHighs - 1)
                        {
                            // Stop reading. Previous data was stop code
                            int outslice = endHighs - 1 + endLows;
                            buffered.RemoveRange(buffered.Count - outslice, outslice);
                            reading = false;
                            lows = 0;
                            highs = 0;
                            PackageDone();
                        }
                        else if (b == HIGH && highs < endHighs - 1)
                        {
                            // Maybe inside stop code, add anyways
                            buffered.Add(b);
                            highs++;
                            reading = true;
                        }
                        else
                        {
                            // Low was data, continue reading. 
                            buffered.Add(b);
                            reading = true;
                            highs = 0;
                            lows = 0;
                        }
                    }
                    else
                    {
                        if (b == LOW)
                        {
                            // Potential end, add anyways
                            buffered.Add(b);
                            reading = true;
                            lows = 1;
                            highs = 0;
                        }
                        else
                        {
                            // Was data, continue reading
                            buffered.Add(b);
                            reading = true;
                            highs = 0;
                            lows = 0;
                        }
                    }
                }
                else
                {
                    if (highs == 1)
                    {
                        if (b == LOW)
                        {
                            // Start reading
                            reading = true;
                            highs = 0;
                            lows = 0;
                        }
                        else
                        {
                            // Reset search
                            reading = false;
                            highs = 0;
                            lows = 0;
                        }
                    }
                    else
                    {
                        if (b == HIGH)
                        {
                            // Has potential start
                            reading = false;
                            highs = 1;
                            lows = 0;
                        }
                        else
                        {
                            // Noise, reset search
                            reading = false;
                            highs = 0;
                            lows = 0;
                        }
                    }
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
