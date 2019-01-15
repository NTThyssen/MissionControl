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
        ServoComponent SV_IPA, SV_N2O, MV_IPA, MV_N2O;
        SolenoidComponent SN_N2O_FILL, SN_FLUSH; 
        TankComponent T_IPA, T_N2O;

        List<StateCommand> _stateCommands;

        public TestStandMapping()
        {
            PT_N2 = new PressureComponent(0, "PT_N2", "PT-N2", x => x);
            PT_IPA = new PressureComponent(0, "PT_IPA", "PT-IPA", x => x);
            PT_N2O = new PressureComponent(0, "PT_N2O", "PT_N2O", x => x);
            PT_FUEL = new PressureComponent(0, "PT_FUEL", "PT_FUEL", x => x);
            PT_OX = new PressureComponent(0, "PT_OX", "PT_OX", x => x);
            PT_CHAM = new PressureComponent(0, "PT_CHAM", "PT_CHAM", x => x);

            TC_IPA = new TemperatureComponent(0, "TC_IPA", "TC_IPA", x => x);
            TC_N2O = new TemperatureComponent(0, "TC_N2O", "TC_N2O", x => x);
            TC_1 = new TemperatureComponent(0, "TC_1", "TC_1", x => x);
            TC_2 = new TemperatureComponent(0, "TC_2", "TC_2", x => x);
            TC_3 = new TemperatureComponent(0, "TC_3", "TC_3", x => x);
            TC_4 = new TemperatureComponent(0, "TC_4", "TC_4", x => x);
            TC_5 = new TemperatureComponent(0, "TC_5", "TC_5", x => x);
            TC_6 = new TemperatureComponent(0, "TC_6", "TC_6", x => x);

            LOAD = new LoadComponent(0, "LOAD", "Load cell", x => x);

            SV_IPA = new ServoComponent(0, "SV_IPA", "SV_IPA", "SV_IPA_SYMBOL");
            SV_N2O = new ServoComponent(0, "SV_N2O", "SV_N2O", "SV_N2O_SYMBOL");
            MV_IPA = new ServoComponent(0, "MV_IPA", "MV_IPA", "MV_IPA_SYMBOL");
            MV_N2O = new ServoComponent(0, "MV_N2O", "MV_N2O", "MV_N2O_SYMBOL");

            SN_N2O_FILL = new SolenoidComponent(0, "SN_N2O_FILL", "SN_N2O_FILL", "SN_N2O_FILL_SYMBOL");
            SN_FLUSH = new SolenoidComponent(0, "SN_FLUSH", "SN_FLUSH", "SN_FLUSH_SYMBOL");

            T_IPA = new TankComponent(0, "FUEL", "TANK-IPA", "FUEL_GRADIENT", 4, 2);
            T_N2O = new TankComponent(0, "OXID", "TANK-N2O", "OXID_GRADIENT", 20, 20);

            T_IPA.Set(1);
            T_N2O.Set(15);

            _stateCommands = new List<StateCommand>
            {
                new StateCommand("First", 0),
                new StateCommand("Second", 1),
                new StateCommand("Third", 2)
            };

        }

        public override List<Component> Components()
        {
            return new List<Component> { PT_N2, PT_IPA, PT_N2O, PT_FUEL, PT_OX, PT_CHAM, TC_IPA, TC_N2O, TC_1, TC_2, TC_3, TC_4, TC_5, TC_6, LOAD, SV_IPA, SV_N2O, SN_FLUSH, SN_N2O_FILL, MV_IPA, MV_N2O, T_IPA, T_N2O };
        }

        public override List<StateCommand> States()
        {
            return _stateCommands;
        }
    }
}
