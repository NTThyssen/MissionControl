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

            PreferenceManager.Manager.Preferences.Fluid.Fuel.CV = 0.0738f;
            PreferenceManager.Manager.Preferences.Fluid.Fuel.Density = 786;

            (mapping.ComponentsByID()[19] as PressureComponent).Set(1087); // 1087.301818 = 24 bar
            (mapping.ComponentsByID()[21] as PressureComponent).Set(611); // 610.6763637 = 8 bar
            (mapping.ComponentsByID()[100] as FlowComponent).Compute();

            float flow = (mapping.ComponentsByID()[100] as FlowComponent).MassFlow;
            Assert.AreEqual(0.06213542364,flow,0.001);
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
            
            p1.Set(670); // 670.2545453 = 10 bar
            fl.Compute();
            tc.Compute(0);
            
            p1.Set(730); // 729.8327273 = 12 bar
            fl.Compute();
            tc.Compute(750);
            
            p1.Set(789); // 789.4109090 = 14 bar
            fl.Compute();
            tc.Compute(1500);
            
            p1.Set(849); // 848.9890910 = 16 bar
            fl.Compute();
            tc.Compute(2250);
            
            p1.Set(909); // 908.5672727 = 18 bar
            fl.Compute();
            tc.Compute(3000);

            float volume = tc.CurrentVolume;
            Assert.AreEqual(3.832606742, volume, 0.1 );
        }
    
    }
}