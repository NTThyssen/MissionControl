using System;
namespace MissionControl.Connection.Commands
{
    public abstract class Command
    {
        public abstract byte[] ToByteData();
    }
}
