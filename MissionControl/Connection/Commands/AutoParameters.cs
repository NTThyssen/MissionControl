using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MissionControl.UI.Widgets;

namespace MissionControl.Connection.Commands
{
    public struct AutoParameters
    {
        private const int N = 15;
        
        public ushort startTime;
        public ushort ignitionTime;
        public ushort fuelState1Time;
        public float fuelState1Percentage;
        public ushort oxidState1Time;
        public float oxidState1Percentage;
        public ushort fuelState2Time;
        public float fuelState2Percentage;
        public ushort oxidState2Time;
        public float oxidState2Percentage;
        public ushort fuelState3Time;
        public float fuelState3Percentage;
        public ushort oxidState3Time;
        public float oxidState3Percentage;
        public ushort endTime;

        public string Serialize()
        {
            string[] s =
            {
                startTime.ToString(),
                ignitionTime.ToString(),
                
                fuelState1Time.ToString(),
                fuelState1Percentage.ToString(CultureInfo.InvariantCulture),
                oxidState1Time.ToString(),
                oxidState1Percentage.ToString(CultureInfo.InvariantCulture),
                
                fuelState2Time.ToString(),
                fuelState2Percentage.ToString(CultureInfo.InvariantCulture),
                oxidState2Time.ToString(),
                oxidState2Percentage.ToString(CultureInfo.InvariantCulture),
                
                fuelState3Time.ToString(),
                fuelState3Percentage.ToString(CultureInfo.InvariantCulture),
                oxidState3Time.ToString(),
                oxidState3Percentage.ToString(CultureInfo.InvariantCulture),
                
                endTime.ToString()
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
                error |= ValidateTime(param[0],"startTime", 0, ref errorMsg, out ap.startTime);
                error |= ValidateTime(param[1], "ignitionTime",ap.startTime, ref errorMsg, out ap.ignitionTime);
            
                error |= ValidateTime(param[2], "fuelState1Time",ap.startTime, ref errorMsg, out ap.fuelState1Time);
                error |= ValidatePosition(param[3], "fuelState1Percentage",ref errorMsg, out ap.fuelState1Percentage);
                error |= ValidateTime(param[4], "oxidState1Time",ap.startTime, ref errorMsg, out ap.oxidState1Time);
                error |= ValidatePosition(param[5],"oxidState1Percentage", ref errorMsg, out ap.oxidState1Percentage);
                
                
                error |= ValidateTime(param[6], "fuelState2Time",ap.fuelState1Time, ref errorMsg, out ap.fuelState2Time);
                error |= ValidatePosition(param[7], "fuelState2Percentage",ref errorMsg, out ap.fuelState2Percentage);
                error |= ValidateTime(param[8], "oxidState2Time",ap.oxidState1Time, ref errorMsg, out ap.oxidState2Time);
                error |= ValidatePosition(param[9],"oxidState2Percentage", ref errorMsg, out ap.oxidState2Percentage);
            
                error |= ValidateTime(param[10], "fuelState3Time",ap.fuelState2Time, ref errorMsg, out ap.fuelState3Time);
                error |= ValidatePosition(param[11], "fuelState3Percentage",ref errorMsg, out ap.fuelState3Percentage);
                error |= ValidateTime(param[12], "oxidState3Time",ap.oxidState2Time, ref errorMsg, out ap.oxidState3Time);
                error |= ValidatePosition(param[13], "oxidState3Percentage",ref errorMsg, out ap.oxidState3Percentage);
                
                error |= ValidateTime(param[14], "endTime",Math.Max(ap.fuelState3Time, ap.oxidState3Time), ref errorMsg, out ap.endTime);

                if (!error)
                {
                    return ap;
                }
            }

            return new AutoParameters();
        }
        
        public static bool ValidateTime(string input, string name, int lowerTime, ref string errMsg, out ushort time)
        {
            if (UInt16.TryParse(input, out ushort result))
            {
                if (result >= lowerTime)
                {
                    time = result;
                    return false;
                }
                
                time = 0;
                errMsg += $"\"{name}\" was smaller than required\n";
                return true;
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
    }
}