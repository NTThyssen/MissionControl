using System;

namespace MissionControl.Connection.Commands
{
    public class Acknowledgement
    {
        public Command Command { get; set; }
        public DateTime Time { get; }
        public byte CommandID { get; set; }

        public Acknowledgement(Command cmd, byte id)
        {
            Command = cmd;
            Time = DateTime.Now;
            CommandID = id;
        }
    }
}