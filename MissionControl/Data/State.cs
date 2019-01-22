using System;
namespace MissionControl.Data
{
    public class State
    {

        public byte StateID { get;}
        public string StateName { get; }
        public State(byte stateID, string name)
        {
            StateID = stateID;
            StateName = name;
        }
    }
}
