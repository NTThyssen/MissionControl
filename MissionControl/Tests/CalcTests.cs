using System;
using MissionControl.Data;
using MissionControl.Data.Components;
using NUnit.Framework;

namespace MissionControl.Tests
{
    [TestFixture]
    public class CalcTests
    {
        [Test]
        public void Validate_Correct_Flow_Calculation()
        {

            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            session.Setting.FuelCV.Value = 0.0738f;
            session.Setting.FuelDensity.Value = 786;

            (mapping.ComponentsByID()[19] as PressureComponent).Set(24);
            (mapping.ComponentsByID()[21] as PressureComponent).Set(8);
            (mapping.ComponentsByID()[100] as FlowComponent).Compute(session.Setting.FuelCV.Value, session.Setting.FuelDensity.Value);

            float flow = (mapping.ComponentsByID()[100] as FlowComponent).MassFlow;
            Assert.IsTrue(Math.Abs(flow - 0.06213542364) < 0.0001);
        }
        
        [Test]
        public void Validate_Correct_Volume_Calculation()
        {
            
            TestStandMapping mapping = new TestStandMapping();
            Session session = new Session(mapping);

            float fuelCV = 0.0738f;
            float fuelDens = 786;

            PressureComponent p1 = (mapping.ComponentsByID()[19] as PressureComponent);
            PressureComponent p2 = (mapping.ComponentsByID()[21] as PressureComponent);
            FlowComponent fl = (mapping.ComponentsByID()[100] as FlowComponent);
            TankComponent tc = (mapping.ComponentsByID()[24] as TankComponent);
            
            tc.SetInputVolume(0, 4);
            p2.Set(0);
            
            p1.Set(10);
            fl.Compute(fuelCV, fuelDens);
            tc.Compute(0);
            
            p1.Set(12);
            fl.Compute(fuelCV, fuelDens);
            tc.Compute(750);
            
            p1.Set(14);
            fl.Compute(fuelCV, fuelDens);
            tc.Compute(1500);
            
            p1.Set(16);
            fl.Compute(fuelCV, fuelDens);
            tc.Compute(2250);
            
            p1.Set(18);
            fl.Compute(fuelCV, fuelDens);
            tc.Compute(3000);

            float volume = tc.CurrentVolume;
            Assert.IsTrue(Math.Abs(volume - 3.832606742) < 0.0001);
        }
    }
}