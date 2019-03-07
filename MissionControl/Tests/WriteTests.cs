using System.Threading;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.UI;
using Moq;
using NUnit.Framework;

namespace MissionControl.Tests
{
    [TestFixture]
    public class WriteTests
    {

        [Test]
        public void Verify_Servo_Command()
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);

            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            ServoCommand sc = new ServoCommand(6, 69.0f);
            
            byte[] actual = {};
                
            Mock<ISerialPort> serial = new Mock<ISerialPort>();
            serial.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] b, int o, int c) =>
                {
                    actual = b;
                });

            ioThread.StartConnection(serial.Object);
            p.OnValvePressed(sc);
            ioThread.WriteAll();
            
            Thread.Sleep(500);
            ioThread.StopConnection();
            
            byte[] expected =
            {
                0xFD, 0xFF, 0xFF, 0xFF, 0xFF,
                0x06, 0xB0, 0xA3,
                0xFE, 0xFF, 0xFF, 0xFF, 0xFF
            };

            Assert.AreEqual(expected, actual);

        }   
        
        [Test]
        public void Verify_Solenoid_Open_Command()
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);

            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            SolenoidCommand sc = new SolenoidCommand(3, true);
            
            byte[] actual = {};
                
            Mock<ISerialPort> serial = new Mock<ISerialPort>();
            serial.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] b, int o, int c) =>
                {
                    actual = b;
                });

            ioThread.StartConnection(serial.Object);
            p.OnValvePressed(sc);
            ioThread.WriteAll();
            
            Thread.Sleep(500);
            ioThread.StopConnection();
            
            byte[] expected =
            {
                0xFD, 0xFF, 0xFF, 0xFF, 0xFF,
                0x03, 0xFF, 0xFF,
                0xFE, 0xFF, 0xFF, 0xFF, 0xFF
            };

            Assert.AreEqual(expected, actual);

        }  
        
        [Test]
        public void Verify_Solenoid_Close_Command()
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);

            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            SolenoidCommand sc = new SolenoidCommand(3, false);
            
            byte[] actual = {};
                
            Mock<ISerialPort> serial = new Mock<ISerialPort>();
            serial.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] b, int o, int c) =>
                {
                    actual = b;
                });

            ioThread.StartConnection(serial.Object);
            p.OnValvePressed(sc);
            ioThread.WriteAll();
            
            Thread.Sleep(500);
            ioThread.StopConnection();
            
            byte[] expected =
            {
                0xFD, 0xFF, 0xFF, 0xFF, 0xFF,
                0x03, 0x00, 0x00,
                0xFE, 0xFF, 0xFF, 0xFF, 0xFF
            };

            Assert.AreEqual(expected, actual);

        }  
    }
}