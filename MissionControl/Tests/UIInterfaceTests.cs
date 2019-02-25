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
            LogThread logThread = new LogThread(dataStore);
            IOThread ioThread = new IOThread(dataStore,ref session);
            
            Program p = new Program(dataStore, logThread, ioThread, ui.Object);
            
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
    }
}