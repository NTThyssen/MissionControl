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
        LabelledEntryWidget ignitionTime;
        LabelledEntryWidget endTime;
        LabelledEntryWidget fuelTimeState1;
        LabelledEntryWidget fuelPercentState1;
        LabelledEntryWidget fuelTimeState2;
        LabelledEntryWidget fuelPercentState2;
        LabelledEntryWidget fuelTimeState3;
        LabelledEntryWidget fuelPercentState3;
        LabelledEntryWidget oxidTimeState1;
        LabelledEntryWidget oxidPercentState1;
        LabelledEntryWidget oxidTimeState2;
        LabelledEntryWidget oxidPercentState2;
        LabelledEntryWidget oxidTimeState3;
        LabelledEntryWidget oxidPercentState3;
   
        private Label headerFuelLabel1;
        private Label headerFuelLabel2;
        private Label headerFuelLabel3;
        private Label headerOxidLabel1;
        private Label headerOxidLabel2;
        private Label headerOxidLabel3;
   

        private Button _btnAutoRunConfigSave;
        public AutoRunConfig() : base(WindowType.Toplevel)
        {
            Title = "Auto Sequence Config";
            
            SetSizeRequest(800, 350);
            SetPosition(WindowPosition.Center);
            Console.WriteLine("hello autoconfig");
            _btnAutoRunConfigSave = new Button{Label = "Save", WidthRequest = 50, HeightRequest = 40};
            headerFuelLabel1 = new Label("Fuel State 1");
            headerFuelLabel2 = new Label("Fuel State 2");
            headerFuelLabel3= new Label("Fuel State 3");
            headerOxidLabel1 = new Label("Oxid State 1");
            headerOxidLabel2 = new Label("Oxid State 2");
            headerOxidLabel3= new Label("Oxid State 3");
            startDelay = new LabelledEntryWidget {
                LabelText = "Start Delay:",
                EntryText =  "",
            };
            fuelTimeState1 = new LabelledEntryWidget
            {
                LabelText =   "Time:",
                EntryText =  "",
                
           
            };
            fuelPercentState1 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
                
            };
            
            fuelTimeState2 = new LabelledEntryWidget
            {
                LabelText = "Time:",
                EntryText = "",
                
            };
            
            fuelPercentState2 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
                
            };
            
            fuelTimeState3 = new LabelledEntryWidget
            {
                LabelText = "Time:",
                EntryText = "",
                
            };
            
            fuelPercentState3 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
               
            };
            
             oxidTimeState1 = new LabelledEntryWidget
            {
                LabelText =   "Time:",
                EntryText =  "",
                
           
            };
            oxidPercentState1 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
                
            };
            
            oxidTimeState2 = new LabelledEntryWidget()
            {
                LabelText = "Time:",
                EntryText = "",
                
                
            };
            
            oxidPercentState2 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
                
            };
            
            oxidTimeState3 = new LabelledEntryWidget
            {
                LabelText = "Time:",
                EntryText = "",
            };
            
            oxidPercentState3 = new LabelledEntryWidget
            {
                LabelText = "Position:",
                EntryText = "",
            };
            
            ignitionTime = new LabelledEntryWidget()
            {
                LabelText = "Ignition Time:",
                EntryText = ""
            };
            
        

            endTime = new LabelledEntryWidget
            {
                LabelText = "End Time",
                EntryText = ""
            };

            VBox container = new VBox(false , 50);
            HBox headerTop = new HBox(false, 0);
            VBox startTimeBox = new VBox(false,0);
            VBox ignitionBox = new VBox(false, 0);
            VBox endTimeBox = new VBox(false,0);
            HBox top = new HBox(false ,0);
            VBox fuelState1 = new VBox(false, 0);
            VBox fuelState2 = new VBox(false, 0);
            VBox fuelState3 = new VBox(false,0);
            HBox bottom = new HBox(false ,0);
            VBox oxidState1 = new VBox(false, 0);
            VBox oxidState2 = new VBox(false, 0);
            VBox oxidState3 = new VBox(false,0);
           
            
            container.PackStart(headerTop, false, false,0);
            container.PackStart(top,false,false,0);
            container.PackStart(bottom,false,false,0);
            top.PackStart(fuelState1, false,false,0);
            top.PackStart(fuelState2,false,false,0);
            top.PackStart(fuelState3, false, false, 0);
            headerTop.PackStart(startTimeBox);
            headerTop.PackStart(ignitionBox);
            headerTop.PackStart(endTimeBox);
           
            startTimeBox.PackStart(startDelay, false, false, 0);
            ignitionBox.PackStart(ignitionTime, false, false, 0);
            endTimeBox.PackStart(endTime, false, false,0);
        
            fuelState1.PackStart(headerFuelLabel1, true,false,0);
            fuelState1.PackStart(fuelTimeState1, false , false,0);
            fuelState1.PackEnd(fuelPercentState1, false ,false,0);
            fuelState2.PackStart(headerFuelLabel2, false, false,0 );
            fuelState2.PackStart(fuelTimeState2,false,false,0);
            fuelState2.PackEnd(fuelPercentState2,false,false,0);
            fuelState3.PackStart(headerFuelLabel3, false, false,0);
            fuelState3.PackStart(fuelTimeState3,false,false,0);
            fuelState3.PackEnd(fuelPercentState3, false,false,0);
            oxidState1.PackStart(headerOxidLabel1, false, false, 0);
            oxidState1.PackStart(oxidTimeState1, false, false, 0);
            oxidState1.PackEnd(oxidPercentState1, false, false, 0);
            
            oxidState2.PackStart(headerOxidLabel2, false, false, 0);
            oxidState2.PackStart(oxidTimeState2, false, false ,0);
            oxidState2.PackEnd(oxidPercentState2, false , false , 0);
            oxidState3.PackStart(headerOxidLabel3, false, false, 0);
            oxidState3.PackStart(oxidTimeState3, false, false, 0);
            oxidState3.PackStart(oxidPercentState3, false ,false, 0);
            bottom.PackStart(oxidState1, false, false, 0);
            bottom.PackStart(oxidState2, false, false, 0);
            bottom.PackStart(oxidState3, false, false,0);
            bottom.PackStart(_btnAutoRunConfigSave, false, false,0);
            _btnAutoRunConfigSave.Clicked += (sender, args) => getTimeings(sender, new EventArgs());
            Add(container);
            ShowAll();
        }
        
        
        
        
        public void getTimeings(object sender, EventArgs e)
        {
           
            List<LabelledEntryWidget> entryValues = new List<LabelledEntryWidget>
            
            {
                startDelay,
                ignitionTime,
                fuelTimeState1,
                fuelPercentState1,
                fuelTimeState2,
                fuelPercentState2,
                fuelTimeState3,
                fuelPercentState3,
                oxidTimeState1,
                oxidPercentState1,
                oxidTimeState2,
                oxidPercentState2,
                oxidTimeState3,
                oxidPercentState3,
                endTime
            };
            int[] intList = new int[15];
            int counter = 0;
            foreach (LabelledEntryWidget label in entryValues)
            {
                Int32.TryParse(label.EntryText, out intList[counter++]);
                Console.WriteLine(intList[counter++].GetType());
                counter++;
            }

                
        }
        
        
    }
  
    
    
}