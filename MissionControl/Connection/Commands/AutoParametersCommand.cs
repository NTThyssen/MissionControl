namespace MissionControl.Connection.Commands
{
    public class AutoParametersCommand : Command
    {
        private ushort _startTime;
        private ushort _ignitionTime;
        private ushort _fuelState1Time;
        private ushort _fuelState1Percentage;
        private ushort _oxidState1Time;
        private ushort _oxidState1Percentage;
        private ushort _fuelState2Time;
        private ushort _fuelState2Percentage;
        private ushort _oxidState2Time;
        private ushort _oxidState2Percentage;
        private ushort _fuelState3Time;
        private ushort _fuelState3Percentage;
        private ushort _oxidState3Time;
        private ushort _oxidState3Percentage;
        private ushort _endTime;
        
        private const byte ID = 0xCB;
        
        public AutoParametersCommand(
            ushort startTime, ushort ignitionTime,
            ushort fuelState1Time, ushort fuelState1Percentage,
            ushort oxidState1Time, ushort oxidState1Percentage,
            ushort fuelState2Time, ushort fuelState2Percentage,
            ushort oxidState2Time, ushort oxidState2Percentage,
            ushort fuelState3Time, ushort fuelState3Percentage,
            ushort oxidState3Time, ushort oxidState3Percentage,
            ushort endTime
            )
        {
            _startTime = startTime;
            _ignitionTime = ignitionTime;
            
            _fuelState1Time = fuelState1Time;
            _fuelState1Percentage = fuelState1Percentage;
            _oxidState1Time = oxidState1Time;
            _oxidState1Percentage = oxidState1Percentage;
            
            _fuelState2Time = fuelState2Time;
            _fuelState2Percentage = fuelState2Percentage;
            _oxidState2Time = oxidState2Time;
            _oxidState2Percentage = oxidState2Percentage;
            
            _fuelState3Time = fuelState3Time;
            _fuelState3Percentage = fuelState3Percentage;
            _oxidState3Time = oxidState3Time;
            _oxidState3Percentage = oxidState3Percentage;
            
            _endTime = endTime;
        }
        
        public override byte[] ToByteData()
        {

            return null;


        }
    }
}