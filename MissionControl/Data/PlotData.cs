using System;
using System.Collections.Generic;
using MissionControl.UI;

namespace MissionControl.Data
{
    public class PlotData
    {
        public List<float> Times { get; set; }
        public Dictionary<string, List<float>> Values { get; set; }

        public PlotData()
        {
            Times = new List<float>();
            Values = new Dictionary<string, List<float>>();
        }

        public Plot.Point[] ToPoints(string sensor) {
            int size = Math.Min(Times.Count, Values[sensor].Count);
            Plot.Point[] points = new Plot.Point[size];

            for(int i = 0; i < size; i++)
            {
                points[i] = new Plot.Point(Times[i], Values[sensor][i]);
            }

            return points;
        }
    }
}
