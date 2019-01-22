using System;
using System.Collections.Generic;
using System.IO;
using MissionControl.UI;

namespace MissionControl.Data
{

    public static class FormatReader
    {

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
