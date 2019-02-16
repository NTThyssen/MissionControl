using System;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public class Session
    {
        public string LogFilePath { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; } = 9600;
        public ComponentMapping Mapping { get; }
        public State State { get; set; }
        public long SystemTime { get; set; }
        public DateTime LastReceived;
        public bool Connected { get; set; }

        public Session(ComponentMapping map)
        {
            Mapping = map;
            State = map.States()[0];
        }

        public void UpdateComponents(byte[] bytes)
        {
            int i = 0;

            while (i < bytes.Length)
            {
                // If special id
                switch (bytes[i])
                {
                    case ComponentMapping.ID_STATE:
                        if (bytes.Length - 1 - i >= ComponentMapping.BC_STATE)
                        {
                            byte state = bytes[i+1];

                            State readState = Mapping.States().Find((State obj) => obj.StateID == state);
                            if (readState != null)
                            {
                                State = readState;
                                Console.WriteLine("State set: {0}", State.StateName);
                            }
                            else
                            {
                                Console.WriteLine("State does not exist in map");
                            }
                            i += 1 + ComponentMapping.BC_STATE;
                            continue;
                        }
                       
                        // Error
                        Console.WriteLine("Not enough value bytes for state");
                        break;
                   
                    case ComponentMapping.ID_TIME:
                        if( bytes.Length - 1 - i >= ComponentMapping.BC_TIME)
                        {
                            byte[] timeBytes = new byte[ComponentMapping.BC_TIME];
                            Array.Copy(bytes, i + 1, timeBytes, 0, ComponentMapping.BC_TIME);
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(timeBytes);
                            }
                            SystemTime = BitConverter.ToUInt32(timeBytes, 0);

                            i += 1 + ComponentMapping.BC_TIME;
                            continue;
                        }
                        // Error
                        Console.WriteLine("Not enough value bytes for time");
                        break;
                }

                // If sensor/component
                // Sign extension with help from: https://stackoverflow.com/questions/3322788/best-practice-for-converting-24bit-little-endian-twos-complement-values-to-in
                if (Mapping.ComponentByIDs().ContainsKey(bytes[i]))
                {
                    Component component = Mapping.ComponentByIDs()[bytes[i]];

                    int size = component.ByteSize;

                    if (bytes.Length - 1 - i >= size)
                    {
                      
                        if (size <= 4)
                        {
                            byte[] valBytes = new byte[4];
                            Array.Copy(bytes, i + 1, valBytes, 4 - size, size);

                            byte sign = (byte) (((valBytes[4 - size] & 0b10000000) == 0) ? 0 : 0xFF);
                            for (int j = 0; j < 4 - size; j++)
                            {
                                valBytes[j] = sign;
                            }

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(valBytes);
                            }
                            int value = BitConverter.ToInt32(valBytes, 0);
                            component.Set(value);
                            //Console.WriteLine("{0} set to {1}", component.Name, value);
                        }
                        else
                        {
                            Console.WriteLine("Byte size is {0}. Can't handle", size);
                        }

                        i += 1 + size;
                        continue;
                    }
                    // Error
                    Console.WriteLine("Not enough value bytes for ID: {0}", bytes[i]);
                    break;
                }

                Console.WriteLine("Unknown ID: {0}", bytes[i]);
                break;

            }
        }


    }
}
