using System;
namespace MissionControl.Connection.Commands
{
    public class StateCommand : Command
    {
        public string StateName { get; } = "NOT SET";
        private readonly int _commandValue;

        public StateCommand(string name, int commandValue)
        {
            StateName = name;
            _commandValue = commandValue;
        }

        public override int CommandValue()
        {
            return _commandValue;
        }
    }
}
