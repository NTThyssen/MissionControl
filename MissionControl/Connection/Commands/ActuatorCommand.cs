using System;
namespace MissionControl.Connection.Commands
{
    public class ActuatorCommand : Command
    {
        private int _percentage;
        private int _boardID;

        public ActuatorCommand(int percentage, int boardID)
        {
            _percentage = percentage;
            _boardID = boardID;
        }

        public override int CommandValue()
        {
            return -1;
        }
    }
}
