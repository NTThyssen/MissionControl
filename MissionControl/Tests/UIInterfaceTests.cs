using System.Collections.Generic;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.UI;
using Moq;
using NUnit.Framework;

namespace MissionControl.Tests
{
    [TestFixture]
    public class UIInterfaceTests
    {
        [Test]
        public void Verify_Stop_Auto_Command_Is_Sent()
        {
            
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);
            
            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            
            p.OnStartAutoSequencePressed();
            p.OnStopAutoSequencePressed();

            List<Command> commands = ioThread.Commands;

            byte[] command1 = commands[0].ToByteData();
            byte[] command2 = commands[1].ToByteData();

            Assert.AreEqual(command1.Length, 2);
            Assert.AreEqual(command2.Length, 2);
            
            Assert.AreEqual(command1[0], 202);
            Assert.AreEqual(command2[0], 202);

            Assert.AreEqual(command1[1], 1);
            Assert.AreEqual(command2[1], 0);
        }
        
        [Test]
        public void Verify_Auto_Params_Sent() {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);
            
            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            
            AutoParameters param = new AutoParameters()
            {
                startTime = 100,
                ignitionTime = 300,
                fuelState1Time = 500,
                fuelState1Percentage = 30,
                oxidState1Time = 500,
                oxidState1Percentage = 40,
                fuelState2Time = 1000,
                fuelState2Percentage = 60,
                oxidState2Time = 1000,
                oxidState2Percentage = 60,
                fuelState3Time = 2000,
                fuelState3Percentage = 100,
                oxidState3Time = 2000,
                oxidState3Percentage = 100,
                endTime = 3000,
            };
            
            p.OnAutoParametersSet(param);
          
            byte[] expected = 
            {
                0xCB,
                0x00, 0x64, 0x01, 0x2C,
                0x01, 0xF4, 0x4C, 0xCD, 0x01, 0xF4, 0x66, 0x66,
                0x03, 0xE8, 0x99, 0x99, 0x03, 0xE8, 0x99, 0x99,
                0x07, 0xD0, 0xFF, 0xFF, 0x07, 0xD0, 0xFF, 0xFF,
                0x0B, 0xB8
            };
            
            Assert.AreEqual(ioThread.Commands[0].ToByteData(), expected);
        }
    }
}