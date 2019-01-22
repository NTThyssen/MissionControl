using System;
namespace MissionControl.Connection.Commands
{
    public class StateCommand : Command
    {
        public readonly byte StateID;
        private const byte ID = 0xC8;

        public StateCommand(byte stateID)
        {
            StateID = stateID;
        }

        public override byte[] ToByteData()
        {
            return new byte[]{ID, StateID };
        }

        public override string ToString()
        {
            return string.Format("StateCommand, StateID: {0}", StateID);
        }
    }
}
