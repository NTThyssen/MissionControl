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
                ap.StartDelay,
                ap.PreStage1Time,
                ap.PreStage2MaxTime,
                ap.PreStage2StableTime,
                ap.RampUpStableTime,
                ap.RampUpMaxTime,
                ap.BurnTime,
                ap.Shutdown1Time,
                ap.Shutdown2Time,
                ap.FlushTime,

                PercentageByte(ap.PreStage1FuelPosition),
                PercentageByte(ap.PreStage2FuelPosition),
                PercentageByte(ap.RampUpFuelPosition),
                PercentageByte(ap.Shutdown1FuelPosition),
                PercentageByte(ap.Shutdown2FuelPosition),
                
                PercentageByte(ap.PreStage1OxidPosition),
                PercentageByte(ap.PreStage2OxidPosition),
                PercentageByte(ap.RampUpOxidPosition),
                PercentageByte(ap.Shutdown1OxidPosition),
                PercentageByte(ap.Shutdown2OxidPosition),
        
                FloatByte(ap.PreStage2StablePressure),
                FloatByte(ap.ChamberPressurePressure),
                FloatByte(ap.EmtpyFuelFeedPressureThreshold),
                FloatByte(ap.EmtpyOxidFeedPressureThreshold)
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