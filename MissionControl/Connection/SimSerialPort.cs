using System;
using System.Collections.Generic;
using System.Linq;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.Connection
{
    public class SimSerialPort : ISerialPort
    {

        private int _index;
        private byte[] _buffer;
        private int _time;
        private ComponentMapping _mapping;

        private DateTime _lastReadFull;

        public SimSerialPort(ComponentMapping mapping)
        {
            _time = 0;
            _index = 0;
            _mapping = mapping;
            _buffer = GenerateFakeBytes(_time, _mapping);
            _lastReadFull = new DateTime(1970, 1, 1);
        }

        public string PortName { get; set; }
        public int BaudRate { get; set; }

        public bool IsOpen { get; set; }

        public int BytesToRead
        {
            get
            {
                if (_buffer.Length - _index == 0)
                {
                    if ((int)(DateTime.Now - _lastReadFull).TotalMilliseconds > 100)
                    {
                        _index = 0;
                        _time++;
                        _buffer = GenerateFakeBytes(_time, _mapping);
                        return _buffer.Length;
                    }
                    return 0;
                }
                return _buffer.Length - _index;
            }
        }

        public void DiscardInBuffer()
        {
            _index = 0;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public int Read(byte[] b, int o, int c)
        {

            int nbytes = Math.Min(c, _buffer.Length - _index);
            Array.Copy(_buffer, _index, b, 0, nbytes);
            _index += nbytes;
            if (_index == _buffer.Length)
            {
                _lastReadFull = DateTime.Now;
            }

            return nbytes;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Console.Write("Writing to serial port: ");
            for (int i = 0; i < count; i++)
            {
                Console.Write("{0:X} ", buffer[i]);
            }
            Console.WriteLine();
        }

        public static byte[] GenerateFakeBytes(int time, ComponentMapping mapping)
        {
            byte[] startNoise = { 0xA, 0xFF, 0xC };
            byte[] startCode = { 0xFF, 0x01 };
            byte[] endCode = { 0x01, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] endNoise = { 0x0B, 0x0C, 0x0D };

            byte[] timeValue = BitConverter.GetBytes(time);
            if (BitConverter.IsLittleEndian) { Array.Reverse(timeValue); }
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
                        value = BitConverter.GetBytes((short)random.Next(0, (int)tank.Full + 1));
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
