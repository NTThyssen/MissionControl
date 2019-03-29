using System;
using System.Collections.Generic;

namespace MissionControl.Data
{
    public static class FormatRaw
    {
        public static readonly byte[] Start =  {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
        public static readonly byte[] End =  {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
        
        public static byte[] ToRaw( byte[] data)
        {
            byte[] wbuffer = new byte[Start.Length + End.Length + data.Length];
            Array.Copy(Start, 0, wbuffer, 0, Start.Length);
            Array.Copy(data, 0, wbuffer, Start.Length, data.Length);
            Array.Copy(End, 0, wbuffer, Start.Length + data.Length, End.Length);
            
            return wbuffer;
        }
        
        public static byte[][] FromRaw(byte[] data)
        {

            byte[] fence = new byte[5];
            bool _reading = false;
            List<byte> _buffered = new List<byte>();
            List<byte[]> frames = new List<byte[]>(); 
            
            foreach (byte b in data)
            {
                
                ShiftArray(fence, b);
                
                if (_reading)
                {
                    _buffered.Add(b);
                    if (CompareArray(fence, Start))
                    {
                        _reading = true;
                        _buffered.Clear();
                    } 
                    else if (CompareArray(fence, End))
                    {
                        _reading = false;
                        int removeIndex = _buffered.Count - End.Length;
                        if (removeIndex > 0)
                        {
                            _buffered.RemoveRange(removeIndex, End.Length);
                            frames.Add(_buffered.ToArray());
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
                    if (CompareArray(fence, Start))
                    {
                        // Ready for new package
                        _reading = true;
                        _buffered.Clear();    
                    } 
                    else if (CompareArray(fence, Start))
                    {
                        _reading = false;
                        _buffered.Clear();
                    }
                }
            }

            return frames.ToArray();
        }

        private static void ShiftArray(byte[] bytes, byte b)
        {
            for (int i = 0; i < bytes.Length - 1; i++)
            {
                bytes[i] = bytes[i + 1];
            }

            bytes[bytes.Length - 1] = b;
        }
        
        private static bool CompareArray(byte[] actual, byte[] expected)
        {
            
            if (actual.Length != expected.Length)
            {
                return false;
            }

            for (int i = 0; i < actual.Length; i++)
            {
                if (actual[i] != expected[i])
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}