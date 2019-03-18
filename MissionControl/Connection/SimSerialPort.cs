using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.Connection
{
    public class SimSerialPort : ISerialPort
    {
        private Queue<byte> _buffer;
        private int _time;
        private ComponentMapping _mapping;

        private Protocol _protocol;

        public SimSerialPort(ComponentMapping mapping)
        {
            _time = 0;
            _mapping = mapping;
            _protocol = new Protocol(PackageHandler);
            _buffer = new Queue<byte>();
            Timer t = new Timer { Interval = 1000 };
            t.Elapsed += (sender, args) =>
            {
               _time += 1000;
               byte[] bytes = GenerateFakeBytes(_time, _mapping);
               lock (_buffer)
               {
                   foreach (byte b in bytes)
                   {
                       _buffer.Enqueue(b);
                   }  
               }
            };
            t.Start();
        }

        private void PackageHandler(Package p)
        {
            Console.WriteLine("Simulated Serial Port received package");
            byte[] bytes = _protocol.GetAcknowledgeWriteBytes(p);
            lock (_buffer)
            {
                foreach (byte b in bytes)
                {
                    _buffer.Enqueue(b);
                }
            }
            
        }

        public string PortName { get; set; }
        public int BaudRate { get; set; }

        public bool IsOpen { get; set; }

        public int BytesToRead => _buffer.Count;

        public void Close()
        {
            IsOpen = false;
        }

        public void DiscardInBuffer()
        {
            _buffer.Clear();
        }

        public void Open()
        {
            IsOpen = true;
        }

        public int Read(byte[] b, int o, int c)
        {
            int nbytes;
            lock (_buffer)
            {
                nbytes = Math.Min(c, _buffer.Count);
                for (int i = 0; i < nbytes; i++)
                {
                    b[i] = _buffer.Dequeue();
                }
      
                /*Console.Write("Simulated sending: ");
                Protocol.PrintArray(b);
                Console.Write("Queue: ");
                Protocol.PrintArray(_buffer.ToArray());
                Console.WriteLine();*/
            }
           
            return nbytes;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Console.Write("Writing to simulated serial port: ");
            Protocol.PrintArray(buffer);
            if (new Random().NextDouble() > 0.5)
            {
                _protocol.Add(buffer);    
            }
            else
            {
                Console.WriteLine("Package was lost!");
            }
            
        }

        public static byte[] GenerateFakeBytes(int time, ComponentMapping mapping)
        {
            byte[] startNoise = { 0xA, 0xB, 0xC, 0xFF, 0xFF };
            byte[] startCode = { 0xFD, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] endCode = { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] endNoise = { 0x0D, 0x0E, 0x0F, 0xFF, 0xFF };

            Random random = new Random();

            byte[] timeValue = BitConverter.GetBytes(time);
            if (BitConverter.IsLittleEndian) { Array.Reverse(timeValue); }
            byte[] btime = new byte[1 + timeValue.Length];
            btime[0] = 0xC9;
            Array.Copy(timeValue, 0, btime, 1, timeValue.Length);

            byte stateValue = (byte) random.Next(0, 9);
            byte[] bstate = { 0xC8, stateValue };
            byte[] bauto = { 0xCA, (byte) random.Next(0, 1) };

            List<byte[]> payload = new List<byte[]>
            {
                btime,
                bstate,
                bauto
            };

            foreach (MeasuredComponent c in mapping.MeasuredComponents())
            {
                byte[] sensor = new byte[1 + c.ByteSize];

                byte[] value = new byte[0];

                switch (c)
                {
                    case PressureComponent pt:
                        value = BitConverter.GetBytes((short)random.Next(400, 1900));
                        break;
                    case TemperatureComponent tc:
                        value = BitConverter.GetBytes((short)random.Next(-50, 300));
                        break;
                    case LoadComponent load:
                        value = BitConverter.GetBytes((short)random.Next(0, 2124));
                        break;
                    case LevelComponent tank:
                        value = BitConverter.GetBytes((short)random.Next(0, (int)tank.Total + 1));
                        break;
                    case ServoComponent servo:
                        ushort randServoVal = (ushort) random.Next(0, ushort.MaxValue);
                        value = BitConverter.GetBytes(randServoVal);
                        break;
                    case ServoTargetComponent servoTarget:
                        ushort randServoTargetVal = (ushort) random.Next(0, ushort.MaxValue);
                        value = BitConverter.GetBytes(randServoTargetVal);
                        break;
                    case SolenoidComponent solenoid:
                        value = BitConverter.GetBytes((short)random.Next(0, 2));
                        break;
                    case VoltageComponent battery:
                        value = BitConverter.GetBytes((ushort)random.Next(12, 14));
                        break;
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(value);
                }

                sensor[0] = c.BoardID;
                Array.Copy(value, 0, sensor, 1, value.Length);
                payload.Add(sensor);
            }
            
            ushort payloadLength = (ushort) payload.SelectMany((byte[] arg) => arg).ToArray().Length;
            
            byte[] header = new byte[4];
            header[0] = 0;
            header[1] = 0;
            byte[] payloadLengthBytes = BitConverter.GetBytes(payloadLength);
            if (BitConverter.IsLittleEndian) { Array.Reverse(payloadLengthBytes); }
            Array.Copy(payloadLengthBytes, 0, header, 2, 2);

            
            payload.Insert(0, startNoise);
            payload.Insert(1, startCode);
            payload.Insert(2, header);
            
            payload.Add(endCode);
            payload.Add(endNoise);
            
            byte[] buffer = payload.SelectMany((byte[] arg) => arg).ToArray();

            return buffer;

        }
    }
}
