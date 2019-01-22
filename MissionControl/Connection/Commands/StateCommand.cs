using System;
namespace MissionControl.Connection.Commands
{
    public class StateCommand : Command
    {
        private readonly int _commandValue;

        public StateCommand(int commandValue)
        {
            _commandValue = commandValue;
        }

        public override int CommandValue()
        {
            return _commandValue;
        }
    }
}
