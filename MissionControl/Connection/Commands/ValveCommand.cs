using System;
namespace MissionControl.Connection.Commands
{
    public abstract class ValveCommand : Command
    {
        protected byte _boardID;

        protected ValveCommand(byte boardID)
        {
            _boardID = boardID;
        }

 
    }
}
