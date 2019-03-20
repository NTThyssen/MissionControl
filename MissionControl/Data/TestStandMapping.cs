using System;
using System.Collections;
using System.Collections.Generic;
using MissionControl.Connection.Commands;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public class TestStandMapping : ComponentMapping
    {
        PressureComponent PT_N2, PT_IPA, PT_N2O, PT_FUEL, PT_OX, PT_CHAM;
        TemperatureComponent TC_IPA, TC_N2O, TC_1, TC_2, TC_3, TC_4, TC_5, TC_6;
        LoadComponent LOAD;
        SolenoidComponent SV_IPA, SV_N2O;
        ServoComponent MV_IPA, MV_N2O;
        ServoTargetComponent TARGET_MV_IPA, TARGET_MV_N2O;
        SolenoidComponent SN_N2O_FILL, SN_FLUSH; 
        LevelComponent T_N2O;
        TankComponent T_IPA;
        VoltageComponent BATTERY;
        FlowComponent FLO_IPA, FLO_N2O;
        private StackHealthComponent STACK_HEALTH;
        

        List<State> _states;

        public TestStandMapping()
        {
            
            PT_N2 = new PressureComponent(16,  "PT_N2", "PT-N2", 400, null);
            PT_IPA = new PressureComponent(17, "PT_IPA", "PT-IPA", 50, null);
            PT_N2O = new PressureComponent(18,  "PT_N2O", "PT-N2O", 50, null);
            PT_FUEL = new PressureComponent(19,  "PT_FUEL", "PT-FUEL", 50, null);
            PT_OX = new PressureComponent(20,  "PT_OX", "PT-OX", 50, null);
            PT_CHAM = new PressureComponent(21, "PT_CHAM", "PT-CHAM", 50, null);

            TC_IPA = new TemperatureComponent(8,  "TC_IPA", "TC-IPA", x => x);
            TC_N2O = new TemperatureComponent(9,  "TC_N2O", "TC-N2O", x => x);
            TC_1 = new TemperatureComponent(10,  "TC_1", "TC-1", x => x);
            TC_2 = new TemperatureComponent(11,  "TC_2", "TC-2", x => x);
            TC_3 = new TemperatureComponent(12,  "TC_3", "TC-3", x => x);
            TC_4 = new TemperatureComponent(13,  "TC_4", "TC-4", x => x);
            TC_5 = new TemperatureComponent(14,  "TC_5", "TC-5", x => x);
            TC_6 = new TemperatureComponent(15,  "TC_6", "TC-6", x => x);
            
            LOAD = new LoadComponent(0, "LOAD", "Load cell");
            
            SV_IPA = new SolenoidComponent(4, "SV_IPA", "SV-IPA", "SV_IPA_SYMBOL");
            SV_N2O = new SolenoidComponent(5, "SV_N2O", "SV-N2O", "SV_N2O_SYMBOL");
            MV_IPA = new ServoComponent(6, "MV_IPA", "MV-IPA", "MV_IPA_SYMBOL");
            MV_N2O = new ServoComponent(7, "MV_N2O", "MV-N2O", "MV_N2O_SYMBOL");

            TARGET_MV_IPA = new ServoTargetComponent(25, "TARGET-MV-IPA");
            TARGET_MV_N2O = new ServoTargetComponent(26, "TARGET-MV-N20");
            
            SN_N2O_FILL = new SolenoidComponent(2, "SN_N2O_FILL", "SN-N2O-FILL", "SN_N2O_FILL_SYMBOL");
            SN_FLUSH = new SolenoidComponent(3,  "SN_FLUSH", "SN-FLUSH", "SN_FLUSH_SYMBOL");

            BATTERY = new VoltageComponent(22,"BATTERY", "BATTERY", 12.0f, 14.8f);

            FLO_IPA = new FlowComponent(100, "FLO_IPA", "FLO-IPA", ref PT_FUEL, ref PT_CHAM, () => PreferenceManager.Manager.Preferences.Fluid.Fuel);
            FLO_N2O = new FlowComponent(101, "FLO_N2O", "FLO-N2O", ref PT_N2O, ref PT_CHAM, () => PreferenceManager.Manager.Preferences.Fluid.Oxid);

            T_IPA = new TankComponent(24, "FUEL", "FUEL", "FUEL_GRADIENT", ref FLO_IPA, "Fuel");
            T_N2O = new LevelComponent(1, "OXID", "OXID", "OXID_GRADIENT", 20);
            
            STACK_HEALTH = new StackHealthComponent(23, "STACK_MAIN", "STACK_ACTUATOR", "STACK_SENSOR", "STACK-HEALTH");
            _states = new List<State>
            {
                new State(0, "Idle"),
                new State(1, "Ignition"),
                new State(2, "Pre-Stage 1"),
                new State(3, "Pre-Stage 2"),
                new State(4, "Ramp up"),
                new State(5, "Regulated"),
                new State(6, "Shutdown 1"),
                new State(7, "Shutdown 2"),
                new State(8, "Flush")
            };
            EmergencyState = _states[5];

        }

        public override List<Component> Components()
        {
            List<Component> components = new List<Component>();
            components.AddRange(MeasuredComponents());
            components.AddRange(ComputedComponents());
            return components;
        }

        public override List<ComputedComponent> ComputedComponents()
        {
            return new List<ComputedComponent> { T_IPA , FLO_IPA, FLO_N2O };
        }

        public override List<MeasuredComponent> MeasuredComponents()
        {
            return new List<MeasuredComponent> { PT_N2, PT_IPA, PT_N2O, PT_FUEL, PT_OX, PT_CHAM, TC_IPA, TC_N2O, TC_1, TC_2, TC_3, TC_4, TC_5, TC_6,
                LOAD, SV_IPA, SV_N2O, SN_FLUSH, SN_N2O_FILL, MV_IPA, MV_N2O, TARGET_MV_IPA, TARGET_MV_N2O, BATTERY, T_N2O, STACK_HEALTH};
        }

        public override List<State> States()
        {
            return _states;
        }
    }
}
