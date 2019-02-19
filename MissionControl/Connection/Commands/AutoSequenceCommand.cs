using System;
namespace MissionControl.Connection.Commands
{
    public class AutoSequenceCommand : Command
    {
        public readonly bool Auto;
        private const byte ID = 0xCA;

        public AutoSequenceCommand(bool auto)
        {
            Auto = auto;
        }

        public override byte[] ToByteData()
        {
            return new byte[] { ID, Auto ? (byte) 1 : (byte) 0 };
        }

        public override string ToString()
        {
            return string.Format("AutoSequenceCommand, Auto: {0}", Auto);
        }
    }
}
