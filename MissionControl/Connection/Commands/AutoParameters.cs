namespace MissionControl.Connection.Commands
{
    public struct AutoParameters
    {
        public ushort startTime;
        public ushort ignitionTime;
        public ushort fuelState1Time;
        public float fuelState1Percentage;
        public ushort oxidState1Time;
        public float oxidState1Percentage;
        public ushort fuelState2Time;
        public float fuelState2Percentage;
        public ushort oxidState2Time;
        public float oxidState2Percentage;
        public ushort fuelState3Time;
        public float fuelState3Percentage;
        public ushort oxidState3Time;
        public float oxidState3Percentage;
        public ushort endTime;
    }
}