using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MissionControl.UI.Widgets;

namespace MissionControl.Connection.Commands
{
    public class AutoParameters
    {
        private const int N = 24;
        
        public ushort StartDelay;
        public ushort IgnitionTime;
        public ushort PreStage1Time;
        public ushort PreStage2MaxTime;
        public ushort PreStage2StableTime;
        public ushort RampUpStableTime;
        public ushort RampUpMaxTime;
        public ushort BurnTime;
        public ushort Shutdown1Time;
        public ushort Shutdown2Time;
        public ushort FlushTime;

        public float PreStage1FuelPosition;
        public float PreStage2FuelPosition;
        public float RampUpFuelPosition;
        public float Shutdown1FuelPosition;
        public float Shutdown2FuelPosition;
        
        public float PreStage1OxidPosition;
        public float PreStage2OxidPosition;
        public float RampUpOxidPosition;
        public float Shutdown1OxidPosition;
        public float Shutdown2OxidPosition;

        public float PreStage2StablePressure;
        public float ChamberPressurePressure;
        public float EmtpyFuelFeedPressureThreshold;
        public float EmtpyOxidFeedPressureThreshold;

        
        public static bool ValidateTime(string input, string name, ref string errMsg, out ushort time)
        {
            if (UInt16.TryParse(input, out ushort result))
            {
                time = result;
                return false;
            }

            time = 0;
            errMsg += $"\"{name}\" was not an integer\n";
            return true;
        }
        
        public static bool ValidatePosition(string input, string name, ref string errMsg, out float position)
        {
            if (float.TryParse(input, out float result))
            {
                if (result >= 0.0 && result <= 100.0)
                {
                    position = result;
                    return false;
                }
                
                position = 0;
                errMsg += $"\"{name}\" was not between 0.0 and 100.0\n";
                return true;
            }
            
            position = 0;
            errMsg += $"\"{name}\" was not a floating point number\n";
            return true;
        }
        
        public static bool ValidatePressure(string input, string name, ref string errMsg, out float pressure)
        {
            if (float.TryParse(input, out float result))
            {
                if (result >= 0.0)
                {
                    pressure = result;
                    return false;
                }
                
                pressure = 0;
                errMsg += $"\"{name}\" was not 0.0 or above\n";
                return true;
            }
            
            pressure = 0;
            errMsg += $"\"{name}\" was not a floating point number\n";
            return true;
        }
    }
}