﻿using System;
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
        ServoComponent SV_IPA, SV_N2O, MV_IPA, MV_N2O;
        SolenoidComponent SN_N2O_FILL, SN_FLUSH; 
        TankComponent T_IPA, T_N2O;
        VoltageComponent BATTERY;

        List<State> _states;


        public TestStandMapping()
        {
            PT_N2 = new PressureComponent(16, 2, "PT_N2", "PT-N2", x => x);
            PT_IPA = new PressureComponent(17, 2, "PT_IPA", "PT-IPA", x => x);
            PT_N2O = new PressureComponent(18, 2, "PT_N2O", "PT-N2O", x => x);
            PT_FUEL = new PressureComponent(19, 2, "PT_FUEL", "PT-FUEL", x => x);
            PT_OX = new PressureComponent(20, 2, "PT_OX", "PT-OX", x => x);
            PT_CHAM = new PressureComponent(21, 2, "PT_CHAM", "PT-CHAM", x => x);

            TC_IPA = new TemperatureComponent(8, 2, "TC_IPA", "TC-IPA", x => x);
            TC_N2O = new TemperatureComponent(9, 2, "TC_N2O", "TC-N2O", x => x);
            TC_1 = new TemperatureComponent(10, 2, "TC_1", "TC-1", x => x);
            TC_2 = new TemperatureComponent(11, 2, "TC_2", "TC-2", x => x);
            TC_3 = new TemperatureComponent(12, 2, "TC_3", "TC-3", x => x);
            TC_4 = new TemperatureComponent(13, 2, "TC_4", "TC-4", x => x);
            TC_5 = new TemperatureComponent(14, 2, "TC_5", "TC-5", x => x);
            TC_6 = new TemperatureComponent(15, 2, "TC_6", "TC-6", x => x);

            LOAD = new LoadComponent(0, 2, "LOAD", "Load cell", x => x);

            SV_IPA = new ServoComponent(4, 2, "SV_IPA", "SV-IPA", "SV_IPA_SYMBOL");
            SV_N2O = new ServoComponent(5, 2, "SV_N2O", "SV-N2O", "SV_N2O_SYMBOL");
            MV_IPA = new ServoComponent(6, 2, "MV_IPA", "MV-IPA", "MV_IPA_SYMBOL");
            MV_N2O = new ServoComponent(7, 2, "MV_N2O", "MV-N2O", "MV_N2O_SYMBOL");

            SN_N2O_FILL = new SolenoidComponent(2, 2, "SN_N2O_FILL", "SN-N2O-FILL", "SN_N2O_FILL_SYMBOL");
            SN_FLUSH = new SolenoidComponent(3, 2, "SN_FLUSH", "SN-FLUSH", "SN_FLUSH_SYMBOL");

            T_IPA = new TankComponent(24, 2, "FUEL", "FUEL", "FUEL_GRADIENT", 4, 2);
            T_N2O = new TankComponent(1, 2, "OXID", "OXID", "OXID_GRADIENT", 20, 20);

            T_IPA.Set(1);
            T_N2O.Set(15);

            BATTERY = new VoltageComponent(22, 2, "BATTERY", "BATTERY", 12.0f, 14.8f);

            _states = new List<State>
            {
                new State(0,"First"),
                new State(1,  "Second"),
                new State(2, "Third")
            };
            EmergencyState = _states[1];

        }

        public override List<Component> Components()
        {
            return new List<Component> { PT_N2, PT_IPA, PT_N2O, PT_FUEL, PT_OX, PT_CHAM, TC_IPA, TC_N2O, TC_1, TC_2, TC_3, TC_4, TC_5, TC_6, LOAD, SV_IPA, SV_N2O, SN_FLUSH, SN_N2O_FILL, MV_IPA, MV_N2O, T_IPA, T_N2O, BATTERY };
        }



        public override List<State> States()
        {
            return _states;
        }
    }
}
