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

    public enum ConnectionStatus
    {
        DISCONNECTED, 
        CONNECTING,
        CONNECTED
    }

    public class IOThread : IIOThread
    {
        private Queue<Command> _commands;
        private Dictionary<byte, Acknowledgement> _waitingForAcknowledment;
        private byte _commandID;

        private const int _headerLength = 4;
        public const int AckWaitMillis = 150;
        
        bool _shouldRun;
        
        Thread t;
        IDataLog _dataLog;
        Session _session;
        ISerialPort _port;
        private Protocol _protocol;

        public List<Command> Commands => _commands.ToList();

        public IOThread(IDataLog dataLog, ref Session session)
        {
            _dataLog = dataLog;
            _commands = new Queue<Command>();
            _waitingForAcknowledment = new Dictionary<byte, Acknowledgement>();
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

        public void RunMethod()
        {
            _commands.Clear();
            _protocol = new Protocol(PackageFound);
            
            while (_shouldRun)
            {
                if (_port.IsOpen)
                {
                    WriteAll();
                    ReadAll();    
                }
                else
                {
                    Open();
                    Thread.Sleep(1000);
                }
            }
            
            _session.Connected = false;
            try
            {
                _port.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Serial IO error in closing: {0}", e.Message);
            }
        }

        private void Open()
        {
            //_session.IsTryingConnect = true;
            try
            {
                _port.Open();
                _port.DiscardInBuffer();
                _session.Connected = true;
                return;
            }
            catch (IOException e)
            {
                Console.WriteLine("Serial IO error in opening: {0}", e.Message);
                _session.Connected = false;
                StopConnection();
                return;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Serial invalid operation error: {0}", e.Message);
            }
            StopConnection();
            _session.Connected = false;
        }

        private void WriteAll()
        {
            Dictionary<byte, Acknowledgement> newAcks = new Dictionary<byte, Acknowledgement>();
            
            foreach (KeyValuePair<byte, Acknowledgement> ack in _waitingForAcknowledment)
            {
                if (DateTime.Now - ack.Value.Time > TimeSpan.FromMilliseconds(AckWaitMillis))
                {
                    Command cmd = ack.Value.Command;
                    Console.Write("Command {0} was not acknowledged fast enough. Resending.", ack.Key);
                    Acknowledgement newAck = WriteCommand(cmd);
                    newAcks.Add(newAck.CommandID, newAck);
                }
                else
                {
                    newAcks.Add(ack.Key, ack.Value);
                }
            }
            
            while (_commands.Count > 0)
            {
                Command cmd = _commands.Dequeue();
                Acknowledgement newAck = WriteCommand(cmd);
                newAcks.Add(newAck.CommandID, newAck);
            }

            _waitingForAcknowledment = newAcks;
        }

        private Acknowledgement WriteCommand(Command cmd)
        {
            _commandID++;
            Acknowledgement ack = new Acknowledgement(cmd, _commandID);

            byte[] wbuffer = _protocol.GetWriteBytes(ack);
            try
            {
                _port.Write(wbuffer, 0, wbuffer.Length);
                Console.Write("Writing: ");
                Protocol.PrintArray(wbuffer);
            }
            catch (IOException)
            {
                Console.WriteLine("While writing an IOException occured");
                StopConnection();
                return null;
            }

            return ack;
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
            _protocol.Add(buf);
        }

        public void PackageFound(Package package)
        {

            if (package.IsAck)
            {
                if (_waitingForAcknowledment.ContainsKey(package.CommandID))
                {
                    Command cmd = _waitingForAcknowledment[package.CommandID].Command;

                    if (Protocol.ArraysAreEqual(cmd.ToByteData(), package.Payload))
                    {
                        // We have received acknowledgement, remove from waiting dictionary
                        Console.WriteLine("Command {0} was acknowledged!", package.CommandID);
                        _waitingForAcknowledment.Remove(package.CommandID);    
                    }
                    else
                    {
                        Console.WriteLine("Acknowledgement for command {0} did not have the correct data", package.CommandID);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("An non-existing command was acknowledged: {0}", package.CommandID);
                }
                
            }
            else
            {
                DataPacket packet = new DataPacket(package.Payload);
                _dataLog.Enqueue(packet);
            }
        }



        

    }
}
