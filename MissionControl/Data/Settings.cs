using System;
using System.Collections.Generic;
using System.Globalization;

namespace MissionControl.Data
{

    public class Property
    {
        public string PreferenceKey { get; }

        public Property(string prefKey)
        {
            PreferenceKey = prefKey;
        }

    }

    public class StringProperty : Property {

        public string Value { get; set; }

        public StringProperty (string prefKey) : base(prefKey) { }

        override public string ToString()
        {
            return Value;
        }
    }

    public class IntegerProperty : Property
    {

        public int Value { get; set; }

        public IntegerProperty (string prefKey) : base(prefKey) { }

        override public string ToString()
        {
            return Convert.ToString(Value);
        }
    }

    public class FloatProperty : Property
    {

        public float Value { get; set; }

        public FloatProperty(string prefKey) : base(prefKey) { }

        override public string ToString()
        {
            return Convert.ToString(Value, CultureInfo.InvariantCulture);
        }
    }

    public class BoolProperty : Property
    {

        public bool Value { get; set; }

        public BoolProperty(string prefKey) : base(prefKey) { }

        override public string ToString()
        {
            return Convert.ToString(Value);
        }
    }

    public class Settings
    {
        // Fluid
        public FloatProperty OxidCV { get; } = new FloatProperty("FLU_OXID_CV");
        public FloatProperty OxidGL { get; } = new FloatProperty("FLU_OXID_GL");
        public FloatProperty FuelCV { get; } = new FloatProperty("FLU_FUEL_CV");
        public FloatProperty FuelGL { get; } = new FloatProperty("FLU_FUEL_GL");
        public FloatProperty TodayPressure { get; } = new FloatProperty("TodayPressure");

        // System
        public StringProperty LogFilePath { get; } = new StringProperty("LogFolder");
        public StringProperty PortName { get; } = new StringProperty("PortName");
        public IntegerProperty BaudRate { get; } = new IntegerProperty("BaudRate");

        // Visual
        public BoolProperty ShowAbsolutePressure { get; } = new BoolProperty("ShowRelativePressure");

        public List<Property> Properties()
        {
            return new List<Property> { LogFilePath, PortName, BaudRate, OxidCV, OxidGL, FuelCV, FuelGL, TodayPressure, ShowAbsolutePressure };
        }

    }

   
}
