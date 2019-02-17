using System;
namespace MissionControl.Data.Components
{
    public interface IWarningLimits
    {
        string PrefMinName { get; }
        string PrefMaxName { get; }

        float MaxLimit { get; set; }
        float MinLimit { get; set; }

        bool IsNominal(float value);
    }
}
