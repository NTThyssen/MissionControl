using System.Net.Sockets;

namespace MissionControl.Connection
{
    public class EthernetPort : ISerialPort
    {
        public bool IsOpen { get; private set; }

        public int BytesToRead => (_stream != null && _stream.DataAvailable) ? 1 : 0;

        private string Address { get; }
        private int Port { get; }

        private TcpClient _tcp;
        private NetworkStream _stream;

        public EthernetPort(string address, int port)
        {
            Address = address;
            Port = port;
            _tcp = new TcpClient();
        }
        public void Open()
        {
            _tcp.Connect(Address, Port);
            _stream = _tcp.GetStream();
            IsOpen = true;
        }

        public void Close()
        {
            _stream?.Close();
            _tcp?.Close();
            IsOpen = false;
        }

        public void DiscardInBuffer(){}

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }
    }
}