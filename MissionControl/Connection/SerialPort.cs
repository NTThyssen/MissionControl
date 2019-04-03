using System;
namespace MissionControl.Connection
{
    public class SerialPort : System.IO.Ports.SerialPort, ISerialPort
    {
        public SerialPort(string portname, int baudrate)
        {
            PortName = portname;
            BaudRate = baudrate;
        }
    }
}
