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
            Assert.IsTrue(Math.Abs(flow - 0.00006213542364) < 0.0001);
        }
    }
}
