using System;
namespace MissionControl.Connection
{
    public interface ISerialPort
    {
        string PortName { get; set; }
        int BaudRate { get; set; }
        bool IsOpen { get; }
        int BytesToRead { get; }

        void Open();
        void DiscardInBuffer();
        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
    }
}
