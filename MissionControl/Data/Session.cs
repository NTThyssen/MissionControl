using System;
namespace MissionControl.Data
{
    public class Session
    {
        private string _logFilePath;
        public ComponentMapping Mapping { get; }

        public Session(string filepath, ComponentMapping map)
        {
            _logFilePath = filepath;
            Mapping = map;
        }



    }
}
