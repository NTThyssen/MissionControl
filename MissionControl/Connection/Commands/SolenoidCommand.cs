using System;
namespace MissionControl.Connection.Commands
{
    public class SolenoidCommand : ValveCommand
    {
        private readonly bool _open;

        public SolenoidCommand(byte boardID, bool open) : base(boardID)
        {
            _open = open;
        }

        public override byte[] ToByteData()
        {
            byte bval = (byte) (_open ? 0xFF : 0x00);
            return new byte[] { _boardID, bval, bval};
        }

        public override string ToString()
        {
            return string.Format("SolenoidCommand, BoardID: {0}, Open: {1}", _boardID, Convert.ToString(_open));
        }
    }
}
