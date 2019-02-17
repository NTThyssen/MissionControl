using System;
using System.Globalization;

namespace MissionControl.Data.Components
{
    public class FlowComponent : ComputedComponent, ILoggable, IWarningLimits
    {

        public string PrefMinName { get { return GraphicID + "_MIN_LIMIT"; } }
        public string PrefMaxName { get { return GraphicID + "_MAX_LIMIT"; } }

        public float MaxLimit { get; set; } = float.NaN;
        public float MinLimit { get; set; } = float.NaN;

        PressureComponent _pressure1, _pressure2;
        public string SettingsConstantName { get; }

        public override string TypeName => "Flow";

        public float MassFlow { get; private set; }

        public FlowComponent(byte boardID, string graphicID, string name, ref PressureComponent p1, ref PressureComponent p2, string settingsConstantName) : base(boardID, graphicID, name)
        {
            _pressure1 = p1;
            _pressure2 = p2;
            SettingsConstantName = settingsConstantName;
        }

        public string LogHeader()
        {
            return Name + " [kg/s]";
        }    

        public string ToLog()
        {
            return (Math.Floor(MassFlow * 100) / 100).ToString(CultureInfo.InvariantCulture);
        }

        public bool IsNominal(float value)
        {
            bool nonNominal = false;
            if (!float.IsNaN(MinLimit)) { nonNominal |= value < MinLimit; }
            if (!float.IsNaN(MaxLimit)) { nonNominal |= value > MaxLimit; }
            return !nonNominal;
        }

        // m^3 / h * (1000 L / m^3) * (h / 3600 s) = 1000 L / 3600 s = 1 L / 3.6 s 
        public void Compute(float cv, float gl)
        {
            float volumeFlow = (float)(cv / (1.17 * Math.Sqrt(gl / Math.Abs(_pressure1.Relative() - _pressure2.Relative()))));
            MassFlow = volumeFlow / 3.6f;
        }
    }
}
