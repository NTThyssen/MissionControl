using System;
namespace MissionControl.Data.Components
{
    public interface ILoggable
    {
        string ToLog();
        string LogHeader();
    }
}
