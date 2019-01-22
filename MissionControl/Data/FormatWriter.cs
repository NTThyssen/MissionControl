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
            int length = 7 + data.Length;
            byte[] stored = new byte[length];
            stored[0] = 0xFF;
            stored[1] = 0x01;
            stored[length - 5] = 0x01;
            stored[length - 4] = 0xFF;
            stored[length - 3] = 0xFF;
            stored[length - 2] = 0xFF;
            stored[length - 1] = 0xFF;
            return stored;
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
