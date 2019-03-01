using System;
using System.Collections.Generic;

namespace MissionControl.Connection.Commands
{
    public class AutoParametersCommand : Command
    {
        private List<ushort> values;
        
        private const byte ID = 0xCB;
        
        public AutoParametersCommand(AutoParameters ap)
        {
            values = new List<ushort>()
            {
                ap.startTime,
                ap.ignitionTime,
                ap.fuelState1Time,
                PercentageByte(ap.fuelState1Percentage),
                ap.oxidState1Time,
                PercentageByte(ap.oxidState1Percentage),
                ap.fuelState2Time,
                PercentageByte(ap.fuelState2Percentage),
                ap.oxidState2Time,
                PercentageByte(ap.oxidState2Percentage),
                ap.fuelState3Time,
                PercentageByte(ap.fuelState3Percentage),
                ap.oxidState3Time,
                PercentageByte(ap.oxidState3Percentage),
                ap.endTime
            };
        }
        
        public override byte[] ToByteData()
        {

            byte[] result = new byte[1 + values.Count * 2];
            result[0] = ID;
            
            for (int i = 0; i < values.Count; i++)
            {
                byte[] val = BitConverter.GetBytes(values[i]);
                if (BitConverter.IsLittleEndian) { Array.Reverse(val); }
                Array.Copy(val, 0, result, 1 + 2 * i, 2);
            }

            return result;
        }

    }
}