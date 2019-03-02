using System;
namespace MissionControl.Connection.Commands
{
    public abstract class Command
    {
        public abstract byte[] ToByteData();
        
        protected ushort PercentageByte(float perc)
        {
            double d = perc / 100.0 * ushort.MaxValue;
            double r = Math.Round(d, MidpointRounding.AwayFromZero);
            return Convert.ToUInt16(r);
        }
        
        protected ushort PercentageByte(int perc)
        {
            double d = perc / 100.0 * ushort.MaxValue;
            double r = Math.Round(d, MidpointRounding.AwayFromZero);
            return Convert.ToUInt16(r);
        }
    }
   
    
    
    
}
