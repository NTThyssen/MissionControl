using System;
using System.Collections.Generic;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public abstract class Component
    {

        public byte BoardID { get; }
        public string GraphicID { get; }
        public string Name { get; }
        public bool Enabled { get; set; } = true;

        public string PrefEnabledName { get { return GraphicID + "_ENABLED"; } }

        public abstract string TypeName { get; }

        public abstract string ToDisplay();

        protected Component(byte boardID, string graphicID, string name)
        {
            BoardID = boardID;
            GraphicID = graphicID;
            Name = name;
        }

        public static string ToRounded(float value, int decimals)
        {
            string rounded = ((float) Math.Round(value, decimals)).ToString(CultureInfo.InvariantCulture);

            return string.Format(CultureInfo.InvariantCulture, "{0:F" + decimals + "}", value);
        }

    }
}
