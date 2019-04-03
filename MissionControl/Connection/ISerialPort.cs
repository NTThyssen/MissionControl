using System;
namespace MissionControl.Connection
{
    public interface ISerialPort
    {
        bool IsOpen { get; }
        int BytesToRead { get; }

        void Open();
        void Close();
        void DiscardInBuffer();
        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
    }
}
