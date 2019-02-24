using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using Gdk;
using Gtk;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI.Widgets;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace MissionControl.UI
{
    public class AutoRunConfig : Window
    {
        
        LabelledEntryWidget StartDelay;
        LabelledEntryWidget FuelTimeState1;
        LabelledEntryWidget FuelPerfcentState1;
        LabelledEntryWidget FuelState2;
        LabelledEntryWidget FuelState3;
        LabelledEntryWidget fuelValveCoefficient;
        LabelledEntryWidget fuelDensity;
        LabelledEntryWidget todaysPressure;
        LabelledRadioWidget showAbsolutePressure;

        private VBox vertical;
        private HBox horizontal;
        private Color _bgColor = new Color(0, 0, 0);
        private Button _btnAutoRunConfigSave;
        public AutoRunConfig() : base(WindowType.Toplevel)
        {
            Title = "Auto Sequence Config";
            
            SetSizeRequest(400, 650);
            SetPosition(WindowPosition.Center);
            Console.WriteLine("hello autoconfig");
            HBox portBox = new HBox(false, 10);
            portBox.PackStart(StartDelay, false ,false, 0);
            Add(portBox);
            ShowAll();
        }
    }
}