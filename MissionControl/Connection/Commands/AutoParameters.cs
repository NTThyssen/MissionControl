using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MissionControl.UI.Widgets;

namespace MissionControl.Connection.Commands
{
    public struct AutoParameters
    {
        private const int N = 24;
        
        public ushort StartDelay;
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

        public string Serialize()
        {
            string[] s =
            {
                StartDelay.ToString(),
                PreStage1Time.ToString(),
                PreStage2MaxTime.ToString(),
                PreStage2StableTime.ToString(),
                RampUpStableTime.ToString(),
                RampUpMaxTime.ToString(),
                BurnTime.ToString(),
                Shutdown1Time.ToString(),
                Shutdown2Time.ToString(),
                FlushTime.ToString(),
        
                PreStage1FuelPosition.ToString(CultureInfo.InvariantCulture),
                PreStage2FuelPosition.ToString(CultureInfo.InvariantCulture),
                RampUpFuelPosition.ToString(CultureInfo.InvariantCulture),
                Shutdown1FuelPosition.ToString(CultureInfo.InvariantCulture),
                Shutdown2FuelPosition.ToString(CultureInfo.InvariantCulture),
                
                PreStage1OxidPosition.ToString(CultureInfo.InvariantCulture),
                PreStage2OxidPosition.ToString(CultureInfo.InvariantCulture),
                RampUpOxidPosition.ToString(CultureInfo.InvariantCulture),
                Shutdown1OxidPosition.ToString(CultureInfo.InvariantCulture),
                Shutdown2OxidPosition.ToString(CultureInfo.InvariantCulture),

                PreStage2StablePressure.ToString(CultureInfo.InvariantCulture),
                ChamberPressurePressure.ToString(CultureInfo.InvariantCulture),
                EmtpyFuelFeedPressureThreshold.ToString(CultureInfo.InvariantCulture),
                EmtpyOxidFeedPressureThreshold.ToString(CultureInfo.InvariantCulture)
            };

            return string.Join(",", s);

        }

        public static AutoParameters Deserialize(string s)
        {
            string[] param = s.Split(',');
            if (param.Length == N)
            {
                AutoParameters ap = new AutoParameters();
                bool error = false;
                string errorMsg = "";
                
                error |= ValidateTime(param[0],nameof(ap.StartDelay), ref errorMsg, out ap.StartDelay);
                error |= ValidateTime(param[0],nameof(ap.PreStage1Time), ref errorMsg, out ap.PreStage1Time);
                error |= ValidateTime(param[0],nameof(ap.PreStage2MaxTime), ref errorMsg, out ap.PreStage2MaxTime);
                error |= ValidateTime(param[0],nameof(ap.PreStage2StableTime), ref errorMsg, out ap.PreStage2StableTime);
                error |= ValidateTime(param[0],nameof(ap.RampUpStableTime), ref errorMsg, out ap.RampUpStableTime);
                error |= ValidateTime(param[0],nameof(ap.RampUpMaxTime), ref errorMsg, out ap.RampUpMaxTime);
                error |= ValidateTime(param[0],nameof(ap.BurnTime), ref errorMsg, out ap.BurnTime);
                error |= ValidateTime(param[0],nameof(ap.Shutdown1Time), ref errorMsg, out ap.Shutdown1Time);
                error |= ValidateTime(param[0],nameof(ap.Shutdown2Time), ref errorMsg, out ap.Shutdown2Time);
                error |= ValidateTime(param[0],nameof(ap.FlushTime), ref errorMsg, out ap.FlushTime);
                
                error |= ValidatePosition(param[0],nameof(ap.PreStage1FuelPosition), ref errorMsg, out ap.PreStage1FuelPosition);
                error |= ValidatePosition(param[0],nameof(ap.PreStage2FuelPosition), ref errorMsg, out ap.PreStage2FuelPosition);
                error |= ValidatePosition(param[0],nameof(ap.RampUpFuelPosition), ref errorMsg, out ap.RampUpFuelPosition);
                error |= ValidatePosition(param[0],nameof(ap.Shutdown1FuelPosition), ref errorMsg, out ap.Shutdown1FuelPosition);
                error |= ValidatePosition(param[0],nameof(ap.Shutdown2FuelPosition), ref errorMsg, out ap.Shutdown2FuelPosition);
                
                error |= ValidatePosition(param[0],nameof(ap.PreStage1OxidPosition), ref errorMsg, out ap.PreStage1OxidPosition);
                error |= ValidatePosition(param[0],nameof(ap.PreStage2OxidPosition), ref errorMsg, out ap.PreStage2OxidPosition);
                error |= ValidatePosition(param[0],nameof(ap.RampUpOxidPosition), ref errorMsg, out ap.RampUpOxidPosition);
                error |= ValidatePosition(param[0],nameof(ap.Shutdown1OxidPosition), ref errorMsg, out ap.Shutdown1OxidPosition);
                error |= ValidatePosition(param[0],nameof(ap.Shutdown2OxidPosition), ref errorMsg, out ap.Shutdown2OxidPosition);

                error |= ValidatePressure(param[0],nameof(ap.PreStage2StablePressure), ref errorMsg, out ap.PreStage2StablePressure);
                error |= ValidatePressure(param[0],nameof(ap.ChamberPressurePressure), ref errorMsg, out ap.ChamberPressurePressure);
                error |= ValidatePressure(param[0],nameof(ap.EmtpyFuelFeedPressureThreshold), ref errorMsg, out ap.EmtpyFuelFeedPressureThreshold);
                error |= ValidatePressure(param[0],nameof(ap.EmtpyOxidFeedPressureThreshold), ref errorMsg, out ap.EmtpyOxidFeedPressureThreshold);
                if (!error)
                {
                    return ap;
                }
            }

            return new AutoParameters();
        }
        
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