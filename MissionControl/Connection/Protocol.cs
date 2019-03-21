using System;
using System.Collections.Generic;
using MissionControl.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using MissionControl.Connection.Commands;
using MissionControl.Data;
namespace MissionControl.Connection
{

    public struct Package
    {
        public bool IsAck { get; set; }
        public byte CommandID { get; set; }
        public ushort PayloadLength { get; set; }
        public byte[] Payload { get; set; }
    }
    
    public class Protocol
    {
        private const int HeaderLength = 4;

        private readonly byte[] _startTag;
        private readonly byte[] _endTag;

        private readonly Queue<byte> _fenceQueue = new Queue<byte>();
        private readonly Queue<byte> _data = new Queue<byte>();
        private readonly List<byte> _buffered = new List<byte>();
        private bool _reading;
        
        public delegate void PackageHandler(Package p);
        private readonly PackageHandler _packageHandler;
        
        public Protocol(PackageHandler packageHandler)
        {
            _startTag = new byte[] {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
            _endTag = new byte[] {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
            _packageHandler = packageHandler;
        }

        public void Add(byte[] data)
        {
            foreach (byte b in data)
            {
                _data.Enqueue(b);    
            }
            CheckForPackage();
        }
        
        private void CheckForPackage()
        {    
            while(_data.Count > 0)
            {
                byte b = _data.Dequeue();
                // Search for starts and ends
                
                AddToFenceQueue(b, _startTag.Length);

                if (_reading)
                {
                    _buffered.Add(b);
                    if (IsFence(_fenceQueue, _startTag))
                    {
                        _reading = true;
                        _buffered.Clear();
                    } 
                    else if (IsFence(_fenceQueue, _endTag))
                    {
                        _reading = false;
                        int removeIndex = _buffered.Count - _endTag.Length;
                        if (removeIndex > 0)
                        {
                            _buffered.RemoveRange(removeIndex, _endTag.Length);
                            BytesToPackage(_buffered.ToArray());
                            _buffered.Clear();
                        }
                        else
                        {
                            // Wrong package
                            _buffered.Clear();
                        }
                    }
                }
                else
                {
                    if (IsFence(_fenceQueue, _startTag))
                    {
                        // Ready for new package
                        _reading = true;
                        _buffered.Clear();    
                    } 
                    else if (IsFence(_fenceQueue, _endTag))
                    {
                        _reading = false;
                        _buffered.Clear();
                    }
                }
            }
        }

        private void BytesToPackage(byte[] bytes)
        {
            if (bytes.Length >= HeaderLength)
            {
                Package package = new Package
                {
                    IsAck = bytes[0] == 1, 
                    CommandID = bytes[1]
                };

                // Compare payload length with the length specified in header
                byte[] lengthBytes = new byte[2];
                Array.Copy(bytes, 2, lengthBytes, 0, 2);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                package.PayloadLength = (ushort) (bytes.Length - HeaderLength);
                ushort payloadLength = BitConverter.ToUInt16(lengthBytes, 0);
                /*
                if (payloadLength != package.PayloadLength)
                {
                    Console.WriteLine("Package did not have correct payload length");
                    return;
                }*/
                
                package.Payload = new byte[package.PayloadLength];
                Array.Copy(bytes, HeaderLength, package.Payload, 0, package.PayloadLength);
                _packageHandler(package);
            }
            else
            {
                Console.WriteLine("Package did not have a correct header");
                PrintArray(bytes);
            }
        }
        
        public byte[] GetWriteSensorBytes(byte[] payload)
        {
            
            byte[] header = new byte[HeaderLength];
            header[0] = 0;
            header[1] = 0;
            
            byte[] payloadLengthBytes = BitConverter.GetBytes((ushort) payload.Length);
            if (BitConverter.IsLittleEndian) { Array.Reverse(payloadLengthBytes); }
            Array.Copy(payloadLengthBytes, 0, header, 2, 2);
            
            byte[] wbuffer = new byte[_startTag.Length + header.Length + payload.Length + _endTag.Length];
            Array.Copy(_startTag, 0, wbuffer, 0, _startTag.Length);
            Array.Copy(header, 0, wbuffer, _startTag.Length, header.Length);
            Array.Copy(payload, 0, wbuffer, _startTag.Length + header.Length, payload.Length);
            Array.Copy(_endTag, 0, wbuffer, _startTag.Length + header.Length + payload.Length, _endTag.Length);

            return wbuffer;
        }
        public byte[] GetWriteBytes(Acknowledgement ack)
        {

            byte[] payload = ack.Command.ToByteData();
            
            byte[] header = new byte[HeaderLength];
            header[0] = 0;
            header[1] = ack.CommandID;
            
            byte[] payloadLengthBytes = BitConverter.GetBytes((ushort) payload.Length);
            if (BitConverter.IsLittleEndian) { Array.Reverse(payloadLengthBytes); }
            Array.Copy(payloadLengthBytes, 0, header, 2, 2);
            
            byte[] wbuffer = new byte[_startTag.Length + header.Length + payload.Length + _endTag.Length];
            Array.Copy(_startTag, 0, wbuffer, 0, _startTag.Length);
            Array.Copy(header, 0, wbuffer, _startTag.Length, header.Length);
            Array.Copy(payload, 0, wbuffer, _startTag.Length + header.Length, payload.Length);
            Array.Copy(_endTag, 0, wbuffer, _startTag.Length + header.Length + payload.Length, _endTag.Length);

            return wbuffer;
        }

        public byte[] GetAcknowledgeWriteBytes(Package package)
        {
            byte[] payload = package.Payload;
            
            byte[] header = new byte[HeaderLength];
            header[0] = 1;
            header[1] = package.CommandID;
            
            byte[] payloadLengthBytes = BitConverter.GetBytes((ushort) payload.Length);
            if (BitConverter.IsLittleEndian) { Array.Reverse(payloadLengthBytes); }
            Array.Copy(payloadLengthBytes, 0, header, 2, 2);
            
            byte[] wbuffer = new byte[_startTag.Length + header.Length + payload.Length + _endTag.Length];
            Array.Copy(_startTag, 0, wbuffer, 0, _startTag.Length);
            Array.Copy(header, 0, wbuffer, _startTag.Length, header.Length);
            Array.Copy(payload, 0, wbuffer, _startTag.Length + header.Length, payload.Length);
            Array.Copy(_endTag, 0, wbuffer, _startTag.Length + header.Length + payload.Length, _endTag.Length);
            
            return wbuffer;
        }

        private void AddToFenceQueue(byte b, int size)
        {
            _fenceQueue.Enqueue(b);
            int dif = Math.Max(_fenceQueue.Count - size, 0);
            for (int i = 0; i < dif; i++)
            {
                _fenceQueue.Dequeue();
            }
        }
        
        private bool IsFence(Queue<byte> actual, byte[] expected)
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
        
        public static void PrintArray(byte[] array)
        {
            foreach (byte b in array)
            {
                Console.Write("{0:X} ", b);
            }
            Console.WriteLine();
        }
        
        public static bool ArraysAreEqual (byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
            {
                return false;
            }
            
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }
        
    }
}