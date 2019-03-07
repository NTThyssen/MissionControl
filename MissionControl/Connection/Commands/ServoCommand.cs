using System;
namespace MissionControl.Connection.Commands
{
    public class ServoCommand : ValveCommand
    {
        private readonly float _percentage;

        public ServoCommand(byte boardID, float percentage) : base(boardID)
        {
            _percentage = Math.Max(Math.Min(100.0f, percentage), 0.0f);
        }

        public override byte[] ToByteData()
        {
            ushort perc = (ushort) (_percentage / 100.0 * ushort.MaxValue);
            byte[] bval = BitConverter.GetBytes(perc);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bval);
            }
            return new byte[] { _boardID, bval[0], bval[1] };
        }

        public override string ToString()
        {
            return string.Format("ServoCommand, BoardID: {0}, Percentage:{1}", _boardID, _percentage);
        }
    }
}
