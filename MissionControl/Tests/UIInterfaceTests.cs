using System.Collections.Generic;
using MissionControl.Connection;
using MissionControl.Connection.Commands;
using MissionControl.Data;
using MissionControl.Data.Components;
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
                StartDelay = 20000, // 0x4E20
                PreStage1Time = 500, // 0x01F4
                PreStage2MaxTime = 3000, // 0x0BB8
                PreStage2StableTime = 250, // 0x00FA
                RampUpStableTime = 250, // 0x00FA
                RampUpMaxTime = 1500, // 0x05DC
                BurnTime = 13000, // 0x32C8
                Shutdown1Time = 500, // 0x01F4
                Shutdown2Time = 500, // 0x01F4
                FlushTime = 2000, // 0x07D0

                PreStage1FuelPosition = 30.0f, // 0x4CCD
                PreStage2FuelPosition = 30.0f, // 0x4CCD
                RampUpFuelPosition = 100.0f, // 0xFFFF
                Shutdown1FuelPosition = 0.0f, // 0x0000
                Shutdown2FuelPosition = 0.0f, // 0x0000
                
                PreStage1OxidPosition = 0.0f, // 0x0000
                PreStage2OxidPosition = 30.0f, // 0x4CCD
                RampUpOxidPosition = 100.0f, // 0xFFFF
                Shutdown1OxidPosition = 0.0f, // 0x0000
                Shutdown2OxidPosition = 0.0f, // 0x0000
        
                PreStage2StablePressure = 4.0f, // 0x0004
                ChamberPressurePressure = 17.0f, // 0x0011
                EmtpyFuelFeedPressureThreshold = 10.0f, // 0x000A
                EmtpyOxidFeedPressureThreshold = 10.0f // 0x000A
            };
            
            p.OnAutoParametersSet(param);
          
            byte[] expected = 
            {
                0xCB,
                
                0x4E, 0x20,
                0x01, 0xF4,
                0x0B, 0xB8,
                0x00, 0xFA,
                0x00, 0xFA,
                0x05, 0xDC,
                0x32, 0xC8,
                0x01, 0xF4,
                0x01, 0xF4,
                0x07, 0xD0,
                
                0x4C, 0xCD,
                0x4C, 0xCD,
                0xFF, 0xFF,
                0x00, 0x00,
                0x00, 0x00,
                0x00, 0x00,
                0x4C, 0xCD,
                0xFF, 0xFF,
                0x00, 0x00,
                0x00, 0x00,
                
                0x00, 0x04,
                0x00, 0x11,
                0x00, 0x0A,
                0x00, 0x0A
            };
            byte[] actual = ioThread.Commands[0].ToByteData();
            Assert.AreEqual(actual, expected);
        }
        
        [Test]
        public void Verify_Tare() {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            DataStore dataStore = new DataStore(session);
            
            Mock<IUserInterface> ui = new Mock<IUserInterface>();
            Mock<LogThread> logThread = new Mock<LogThread>(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);
            
            Program p = new Program(dataStore, logThread.Object, ioThread, ui.Object);
            
            LoadComponent l = (mapping.ComponentsByID()[0] as LoadComponent);
            
            Assert.AreEqual(0,l.Newtons());
            l.Set(10);
            Assert.AreEqual(10 * l.Gravity,l.Newtons());
            l.Tare();
            l.Set(10);
            Assert.AreEqual(0,l.Newtons());
            l.Set(200);
            Assert.AreEqual(200 * l.Gravity - 10 * l.Gravity,l.Newtons());
        }
    }
}