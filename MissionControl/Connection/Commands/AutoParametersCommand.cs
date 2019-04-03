using System;
using System.Collections.Generic;
using MissionControl.Data;
using MissionControl.Data.Components;

namespace MissionControl.Connection.Commands
{
    public class AutoParametersCommand : Command
    {
        private List<ushort> values;
        
        private const byte ID = 0xCB;
        
        public AutoParametersCommand(AutoParameters ap, ComponentMapping mapping)
        {
            Dictionary<byte, Component> components = mapping.ComponentsByID();
            PressureComponent chamber = (PressureComponent)
                components[PreferenceManager.Manager.Preferences.AutoSequenceComponentIDs.ChamberPressureID];
            PressureComponent fuelLine = (PressureComponent)
                components[PreferenceManager.Manager.Preferences.AutoSequenceComponentIDs.ChamberPressureID];
            PressureComponent oxidLine = (PressureComponent)
                components[PreferenceManager.Manager.Preferences.AutoSequenceComponentIDs.ChamberPressureID];
            
            values = new List<ushort>()
            {
                ap.StartDelay,
                ap.IgnitionTime,
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
        
                PressureByte(ap.PreStage2StablePressure, chamber),
                PressureByte(ap.ChamberPressurePressure, chamber),
                PressureByte(ap.EmptyFuelFeedPressureThreshold, fuelLine),
                PressureByte(ap.EmptyOxidFeedPressureThreshold, oxidLine)
            };
        }

        public ushort PressureByte(float val, PressureComponent component)
        {
            return FloatByte(component.UncalibratedValue(val));
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