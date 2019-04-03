using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public static class FormatPretty
    {
     
        public static string PrettyHeader(List<ILoggable> components)
        {
            StringBuilder sb = new StringBuilder();

            string sep = (components.Count > 0) ? "," : string.Empty;
            sb.Append("Time" + sep);

            for (int i = 0; i < components.Count; i++)
            {
                sep = (i < components.Count - 1) ? "," : string.Empty;
                sb.Append(components[i].LogHeader() + sep);
            }

            return sb.ToString();
        }

        public static string PrettyLine(long time, List<ILoggable> components)
        {
            StringBuilder sb = new StringBuilder();

            string sep = (components.Count > 0) ? "," : string.Empty;
            sb.Append(time + sep);

            for (int i = 0; i < components.Count; i++)
            {
                sep = (i < components.Count - 1) ? "," : string.Empty;
                string logval = components[i].ToLog();
                sb.Append(logval + sep);
            }

            return sb.ToString();
        }

        public static PlotData PrettyToData (StreamReader file) {

            PlotData data = new PlotData();

            string headerLine = file.ReadLine();

            if (headerLine == null)
            {
                // Empty
                return data;
            }

            string[] headers = headerLine.Split(',');

            // Skip time
            for (int i = 1; i < headers.Length; i++)
            {
                data.Values[headers[i]] = new List<float>();
            }

            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split(',');

                float time;
                try
                {
                    time = Convert.ToSingle(values[0]);
                } catch (Exception e)
                {
                    Console.WriteLine("Error in converting time: {0}", e.Message);
                    continue;
                }

                data.Times.Add(time);

                // Skip time
                for (int i = 1; i < values.Length; i++)
                {
                    float value;
                    try
                    {
                        value = Convert.ToSingle(values[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in converting time: {0}", e.Message);
                        continue;
                    }

                    data.Values[headers[i]].Add(value);

                }
            }


            return data;
        }
        
    }
}
