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
using Svg;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace MissionControl.UI
{
    public class AutoRunConfig : Window
    {
        
        LabelledEntryWidget startDelay;
        LabelledEntryWidget fuelTimeState1;
        LabelledEntryWidget fuelPerfcentState1;
        LabelledEntryWidget fuelState2;
        LabelledEntryWidget fuelState3;
        LabelledEntryWidget fuelValveCoefficient;
        LabelledEntryWidget fuelDensity;
        LabelledEntryWidget todaysPressure;
        LabelledEntryWidget showAbsolutePressure;
        private Label sequence;
        
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
            _btnAutoRunConfigSave = new Button{Label = "Save", WidthRequest = 50, HeightRequest = 40};
            sequence = new Label("auto sequence");
            startDelay = new LabelledEntryWidget {
                LabelText = "Start Delay",
                EntryText =  ""
            };
            
            VBox headerBox = new VBox( false, 10 );
            
            HBox horiBox = new HBox(false, 10);
            VBox vertBox = new VBox(false , 10);
           
            vertBox.PackStart(startDelay, false, false, 0);
            horiBox.PackStart(vertBox, false, false, 0);
            horiBox.PackStart(_btnAutoRunConfigSave, false ,false, 0);
            headerBox.PackStart(sequence, false, false,0);
            headerBox.PackStart(horiBox, false, false, 0);
            Add(headerBox);
            ShowAll();
        }
    }
}