using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
using System.Linq;

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
        Session _session;
        ISerialPort _port;
        bool _shouldRun;

        public IOThread(IDataLog dataLog, ref Session session, ISerialPort serialPort)
        {
            t = new Thread(RunMethod);
            t.Name = "IO Thread";
            _dataLog = dataLog;
            _commands = new Queue<Command>();
            _session = session;
            _port = serialPort;
        }


        public void StartThread() {
            _shouldRun = true;
            t.Start(); 
        }

        public void StopThread() {
            _shouldRun = false;
            Console.Write("Thread stopping");
            t.Join(2000);
         }

        public void SendCommand(Command cmd)
        {
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
        const int endHighs = 4;
        const int endLows = 1;
        const int bufsize = 8;
        int highs, lows;

        //byte[] fakeBuffer = { 0xA, 0xFF, 0xC, 0xFF, 0x01, 0x14, 0x0B, 0xD0, 0xC8, 0x02, 0xC9, 0x00, 0x00, 0x27, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xA };
        //byte[] fakeBuffer = GenerateFakeBytes(t, _session.Mapping);

        public void RunMethod()
        {
            Console.WriteLine("Thread run started");
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

            _port.BaudRate = 9600;
            try
            {
                _port.Open();
                _port.DiscardInBuffer();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Serial IO error: {0}", e.Message);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Serial invalid operation error: {0}", e.Message);
            }
           
        }

        private void WriteAll()
        {
            while (_commands.Count > 0)
            {


                Console.WriteLine("Command: {0}", _commands.Dequeue().CommandValue());
            }
        }

        private void ReadAll()
        {
            //Console.Write("Read start");
            //Thread.Sleep(10);
            while (_port.BytesToRead >= bufsize && _shouldRun)
            {
                byte[] buf = new byte[bufsize];
                int bytesRead = _port.Read(buf, 0, bufsize);
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

        public static byte[] GenerateFakeBytes(int time, ComponentMapping mapping) {
            byte[] startNoise = { 0xA, 0xFF, 0xC };
            byte[] startCode = { 0xFF, 0x01};
            byte[] endCode = { 0x01, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] endNoise = { 0x0B, 0x0C, 0x0D };

            byte[] timeValue = BitConverter.GetBytes(time);
            if(BitConverter.IsLittleEndian) { Array.Reverse(timeValue); }
            byte[] btime = new byte[1 + timeValue.Length];
            btime[0] = 0xC9;
            Array.Copy(timeValue, 0, btime, 1, timeValue.Length);

            byte stateValue = (byte)new Random().Next(0, 3);
            byte[] bstate = { 0xC8, stateValue };

            List<byte[]> data = new List<byte[]>
            {
                startNoise,
                startCode,
                btime,
                bstate
            };

            Random random = new Random();

            foreach (Component c in mapping.Components())
            {
                byte[] sensor = new byte[1 + c.ByteSize];

                byte[] value = new byte[0];

                switch (c)
                {
                    case PressureComponent pt:
                        value = BitConverter.GetBytes((short)random.Next(0, 40));
                        break;
                    case TemperatureComponent tc:
                        value = BitConverter.GetBytes((short)random.Next(-50, 300));
                        break;
                    case LoadComponent load:
                        value = BitConverter.GetBytes((short)random.Next(0, 311));
                        break;
                    case TankComponent tank:
                        value = BitConverter.GetBytes((short)random.Next(0, (int) tank.Full +1));
                        break;
                    case ServoComponent servo:
                        value = BitConverter.GetBytes((short)random.Next(0, 101));
                        break;
                    case SolenoidComponent solenoid:
                        value = BitConverter.GetBytes((short)random.Next(0, 2));
                        break;
                    case VoltageComponent battery:
                        value = BitConverter.GetBytes((short)random.Next(12, 14));
                        break;
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(value);
                }

                sensor[0] = c.BoardID;
                Array.Copy(value, 0, sensor, 1, value.Length);
                data.Add(sensor);
            }

            data.Add(endCode);
            data.Add(endNoise);

            byte[] buffer = data.SelectMany((byte[] arg) => arg).ToArray();

            return buffer;

        }
    }
}
