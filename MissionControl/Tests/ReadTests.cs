using System;
using MissionControl.Data;
using NUnit.Framework;
using Moq;
using MissionControl.Connection;
using MissionControl.Data.Components;
using System.Threading;

namespace MissionControl.Tests
{
    [TestFixture]
    public class ReadTests
    {

        [Test]
        public void Single_Component_Is_Updated_By_DataPacket ()
        {
            byte ID = 0;
            // Value = 1337 = 0x539
            byte valMSB = 0x05;
            byte valLSB = 0x39;

            LoadComponent expectedResult = new LoadComponent(0, 2, "", "", x => x);
            expectedResult.Set(1337);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            byte[] buffer = { ID, valMSB, valLSB };
            session.UpdateComponents(buffer);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentByIDs()[ID]).Newtons());
        }

        [Test]
        public void Single_Component_Is_Updated_After_Incoming_Data_Without_Logging() 
        {
            byte ID = 0;
            // Value = 1337 = 0x539
            byte valMSB = 0x05;
            byte valLSB = 0x39;

            LoadComponent expectedResult = new LoadComponent(0, 2, "", "", x => x);
            expectedResult.Set(1337);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            Mock<IDataLog> dataMock = new Mock<IDataLog>();


            Mock<ISerialPort> serialMock = new Mock<ISerialPort>();
            byte[] buffer = { 0xAA, 0xBB, 0xFF, 0x01, ID, valMSB, valLSB, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xAA, 0xBB };
            int i = 0;

            serialMock.Setup(x => x.IsOpen).Returns(true).Callback(() => Console.WriteLine("IsOpen called"));
            serialMock.Setup(x => x.BytesToRead).Returns(buffer.Length - i).Callback(() => Console.WriteLine("BytesToRead called"));
            serialMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((byte[] b, int o, int c) => {
                    int nbytes = Math.Min(c, buffer.Length - i);
                    Array.Copy(buffer, i, b, 0, nbytes);
                    i += nbytes;
                    i = (i >= buffer.Length) ? 0 : i;
                    Console.WriteLine("Got {0} bytes", nbytes);
                    return nbytes;
                }).Callback(() => Console.WriteLine("Read called"));

            bool wait = true;

            IOThread conn = new IOThread(dataMock.Object, ref session, serialMock.Object);
            dataMock.Setup(x => x.Enqueue(It.IsAny<DataPacket>())).Callback((DataPacket dp) => {
                session.UpdateComponents(dp.Bytes);
                conn.StopThread();
                wait = false;
            });
            conn.StartThread();
            Console.WriteLine("Thread started");
            while (wait)
            {
                Thread.Sleep(100);
            }
            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentByIDs()[ID]).Newtons());
            //conn.StopThread();
            //dataMock.Verify(x => x.Enqueue(It.IsAny<DataPacket>()), Times.AtLeastOnce);


        }

     

    }
}
