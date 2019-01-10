using System;
namespace MissionControl.Data
{
    public class Session
    {
        private string _filepath;
        private ComponentMapping _componentsMap;

        public Session(string filepath, ComponentMapping map)
        {
            _filepath = filepath;
            _componentsMap = map;
        }
    }
}
