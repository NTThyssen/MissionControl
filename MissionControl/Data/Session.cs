using System;
using MissionControl.Connection;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public class Session
    {
    
        public ComponentMapping Mapping { get; }
        public State State { get; set; }
        public long SystemTime { get; set; }
        public DateTime LastReceived { get; set; } = DateTime.Now - TimeSpan.FromMinutes(1);
        public int QueueSize { get; set; }
        public bool Connected { get; set; }

        public ConnectionStatus ConnectionStatus;
        //public Settings Setting { get; set; }
        public bool IsAutoSequence { get; set; } = false;

        public Session(ComponentMapping map)
        {
            Mapping = map;
            State = map.States()[0];
            //Setting = new Settings();
           
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

                    case ComponentMapping.ID_AUTO:
                        if (bytes.Length - 1 - i >= ComponentMapping.BC_AUTO)
                        {
                            byte isAuto = bytes[i + 1];

                            IsAutoSequence = isAuto > 0; 

                            i += 1 + ComponentMapping.BC_AUTO;
                            continue;
                        }
                        // Error
                        Console.WriteLine("Not enough value bytes for time");
                        break;
                }

                // If sensor/component
                // Sign extension with help from: https://stackoverflow.com/questions/3322788/best-practice-for-converting-24bit-little-endian-twos-complement-values-to-in
                if (Mapping.ComponentsByID().ContainsKey(bytes[i]))
                {
                    if (Mapping.ComponentsByID()[bytes[i]] is MeasuredComponent component)
                    {
                        int size = component.ByteSize;

                        if (bytes.Length - 1 - i >= size)
                        {

                            if (size <= 4)
                            {
                                byte[] valBytes = new byte[4];
                                Array.Copy(bytes, i + 1, valBytes, 4 - size, size);

                                if (component.Signed)
                                {
                                    byte sign = (byte)(((valBytes[4 - size] & 0b10000000) == 0) ? 0 : 0xFF);
                                    for (int j = 0; j < 4 - size; j++)
                                    {
                                        valBytes[j] = sign;
                                    }
                                }

                                if (BitConverter.IsLittleEndian)
                                {
                                    Array.Reverse(valBytes);
                                }
                                
                                if (component.Signed)
                                {
                                    int value = BitConverter.ToInt32(valBytes, 0);
                                    component.Set(value);    
                                }
                                else
                                {
                                    //Console.WriteLine("Name: {0}, Bytes {1} {2} {3} {4}", component.Name, valBytes[3], valBytes[2], valBytes[1], valBytes[0]);
                                    uint value = BitConverter.ToUInt32(valBytes, 0);
                                    //Console.WriteLine("Name: {0}, uint: {1} int: {2}", component.Name, value, (int) value);
                                    component.Set((int) value);
                                }
                                
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
                }

                Console.WriteLine("Unknown ID: {0}", bytes[i]);
                break;

            }

            foreach(ComputedComponent c in Mapping.ComputedComponents())
            {
                switch (c)
                {
                    case FlowComponent fc:
                        fc.Compute();
                        break;
                    case TankComponent tc:
                        tc.Compute(SystemTime);
                        break;
                }
            }
        }


    }
}
