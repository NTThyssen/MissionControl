using System;
using System.Collections.Generic;
using MissionControl.Data;

namespace RawExtractor
{
   
    public class Protocol
    {

        private byte[] _startTag;
        private byte[] _endTag;

        private Queue<byte> _fenceQueue = new Queue<byte>();
        private Queue<byte> _data = new Queue<byte>();
        private bool _reading;
        private List<byte> _buffered = new List<byte>();
        
        public Protocol(byte[] startTag, byte[] endTag)
        {
            _startTag = startTag;
            _endTag = endTag;
        }

        public void Add(byte[] data)
        {
            foreach (byte b in data)
            {
                _data.Enqueue(b);    
            }
        }
        
        
        public List<DataPacket> FindFrames()
        {
            List<DataPacket> frames = new List<DataPacket>(); 
            
            while (_data.Count > 0)
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
                            DataPacket packet = new DataPacket(_buffered.ToArray());
                            frames.Add(packet);
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

            return frames;
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
        
    }
}