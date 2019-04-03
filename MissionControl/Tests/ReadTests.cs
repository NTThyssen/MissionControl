using System;
using System.Diagnostics;
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
        public void Temperature_Component_Is_Updated_Correctly ()
        {
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            
            const byte ID = 10;

            // Value = -200 = 0xFF38
            byte valMSB = 0xFF;
            byte valLSB = 0x38;
            byte[] buffer = { ID, valMSB, valLSB };
            
            TemperatureComponent expectedResult = new TemperatureComponent(0, "", "", x => x);
            session.UpdateComponents(buffer);
            expectedResult.Set(-200);

            Assert.AreEqual(expectedResult.Celcius(), ((TemperatureComponent) mapping.ComponentsByID()[ID]).Celcius());
            
            // Value = 200 = 0x00C8
            valMSB = 0x00;
            valLSB = 0xC8;
            buffer = new []{ ID, valMSB, valLSB };
            
            session.UpdateComponents(buffer);
            expectedResult.Set(200);

            Assert.AreEqual(expectedResult.Celcius(), ((TemperatureComponent) mapping.ComponentsByID()[ID]).Celcius());
        }
        
        [Test]
        public void Pressure_Component_Is_Updated_Correctly ()
        {
            byte ID = 16;
            PressureComponent expectedResult = new PressureComponent(0, "", "", 400);
            expectedResult.Set(40000);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            // Value = 1337 = 0x539
            session.UpdateComponents(new byte[]{ ID, 0x9C, 0x40 });

            Assert.AreEqual(expectedResult.Relative(), ((PressureComponent) mapping.ComponentsByID()[ID]).Relative());
        }
        
        [Test]
        public void Servo_Component_Is_Updated_Correctly ()
        {
            byte ID = 6;
            ServoComponent expectedResult = new ServoComponent(0, "", "", "");
            expectedResult.Set(40000);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            // Value = 1337 = 0x539
            session.UpdateComponents(new byte[]{ ID, 0x9C, 0x40 });

            Assert.AreEqual(expectedResult.Degree(), ((ServoComponent) mapping.ComponentsByID()[ID]).Degree());
        }
        
        [Test]
        public void Load_Component_Is_Updated_Correctly ()
        {
            
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);
            
            const byte ID = 0;

            // Value = -200 = 0xFF38
            byte valMSB = 0xFF;
            byte valLSB = 0x38;
            byte[] buffer = { ID, valMSB, valLSB };
            
            LoadComponent expectedResult = new LoadComponent(0, "", "");
            session.UpdateComponents(buffer);
            expectedResult.Set(-200);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent) mapping.ComponentsByID()[ID]).Newtons());
            
            // Value = 200 = 0x00C8
            valMSB = 0x00;
            valLSB = 0xC8;
            buffer = new []{ ID, valMSB, valLSB };
            
            session.UpdateComponents(buffer);
            expectedResult.Set(200);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent) mapping.ComponentsByID()[ID]).Newtons());
        }
        
        [Test]
        public void Single_Component_Is_Updated_By_DataPacket ()
        {
            byte ID = 0;
            // Value = 1337 = 0x539
            byte valMSB = 0x05;
            byte valLSB = 0x39;

            LoadComponent expectedResult = new LoadComponent(0, "", "");
            expectedResult.Set(1337);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            byte[] buffer = { ID, valMSB, valLSB };
            session.UpdateComponents(buffer);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentsByID()[ID]).Newtons());
        }

        [Test]
        public void Single_Component_Is_Updated_By_DataPacket_Verify_Sign_Extension()
        {
            byte ID = 0;
            // Value = 250 = 0x00FA
            byte valMSB = 0x00;
            byte valLSB = 0xFA;

            LoadComponent expectedResult = new LoadComponent(0, "", "");
            expectedResult.Set(250);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            byte[] buffer = { ID, valMSB, valLSB };
            session.UpdateComponents(buffer);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentsByID()[ID]).Newtons());
        }

        [Test]
        public void Single_Component_Is_Updated_By_DataPacket_With_Negative_Value()
        {
            byte ID = 0;
            // Value = -1337 = 0xFAC7
            byte valMSB = 0xFA;
            byte valLSB = 0xC7;

            LoadComponent expectedResult = new LoadComponent(0,"", "");
            expectedResult.Set(-1337);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            byte[] buffer = { ID, valMSB, valLSB };
            session.UpdateComponents(buffer);

            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentsByID()[ID]).Newtons());
        }

        [Test]
        public void Single_Component_Is_Updated_After_Incoming_Data_Without_Logging() 
        {
            byte ID = 0;
            // Value = 1337 = 0x539
            byte valMSB = 0x05;
            byte valLSB = 0x39;

            LoadComponent expectedResult = new LoadComponent(0, "", "");
            expectedResult.Set(1337);

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            Mock<IDataLog> dataMock = new Mock<IDataLog>();


            Mock<ISerialPort> serialMock = new Mock<ISerialPort>();
            byte[] buffer = { 0xAA, 0xBB, 0xFD, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x03, ID, valMSB, valLSB, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xAA, 0xBB };
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

            IOThread conn = new IOThread(dataMock.Object, ref session);
            dataMock.Setup(x => x.Enqueue(It.IsAny<DataPacket>())).Callback((DataPacket dp) => {
                session.UpdateComponents(dp.Bytes);
                conn.StopConnection();
                wait = false;
            });
            conn.StartConnection(serialMock.Object, null);
            Console.WriteLine("Thread started");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (wait && watch.ElapsedMilliseconds < 2000)
            {
                Thread.Sleep(100);
            }
            Assert.AreEqual(expectedResult.Newtons(), ((LoadComponent)mapping.ComponentsByID()[ID]).Newtons());

        }
    }
}
