using System;
using System.Collections.Generic;
using System.Text;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public static class FormatWriter
    {

        public static byte[] ToRaw( byte[] data)
        {
            byte[] start = new byte[] {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
            byte[] end   = new byte[] {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
            byte[] wbuffer = new byte[start.Length + end.Length + data.Length];
            Array.Copy(start, 0, wbuffer, 0, start.Length);
            Array.Copy(data, 0, wbuffer, start.Length, data.Length);
            Array.Copy(end, 0, wbuffer, start.Length + data.Length, end.Length);
            return wbuffer;
        }

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

        public static string PrettyLine(long time, byte[] data, List<ILoggable> components)
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

    }
}
