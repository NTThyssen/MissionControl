using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using Castle.Core.Internal;
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
   

        private Button _btnAutoRunConfigSave;
        
        public AutoRunConfig() : base(WindowType.Toplevel)
        {
            Title = "Auto Sequence Config";
            
            SetSizeRequest(950, 350);
            SetPosition(WindowPosition.Center);
            Console.WriteLine("hello autoconfig");
            _btnAutoRunConfigSave = new Button{Label = "Save", WidthRequest = 80, HeightRequest = 40};
            startDelay = new LabelledEntryWidget {
                LabelText = "Start Delay:",
                EntryText =  "",
            };
            fuelTimeState1 = new LabelledEntryWidget
            {
                LabelText =   "Time Fuel State1:",
                EntryText =  "",
                    
           
            };
            
            fuelPercentState1 = new LabelledEntryWidget
            {
                LabelText = "Position Fuel State1:",
                EntryText = "",
                
            };
            
            fuelTimeState2 = new LabelledEntryWidget
            {
                LabelText = "Time Fuel State2:",
                EntryText = "",
                
            };
            
            fuelPercentState2 = new LabelledEntryWidget
            {
                LabelText = "Position Fuel State2:",
                EntryText = "",
                
            };
            
            fuelTimeState3 = new LabelledEntryWidget
            {
                LabelText = "Time Fuel State3:",
                EntryText = "",
                
            };
            
            fuelPercentState3 = new LabelledEntryWidget
            {
                LabelText = "Position Fuel State3:",
                EntryText = "",
               
            };
            
             oxidTimeState1 = new LabelledEntryWidget
            {
                LabelText =   "Time Oxid State1:",
                EntryText =  "",
                
           
            };
            oxidPercentState1 = new LabelledEntryWidget
            {
                LabelText = "Position Oxid State1:",
                EntryText = "",
                
            };
            
            oxidTimeState2 = new LabelledEntryWidget()
            {
                LabelText = "Time Oxid State2:",
                EntryText = "",
                
                
            };
            
            oxidPercentState2 = new LabelledEntryWidget
            {
                LabelText = "Position Oxid State2:",
                EntryText = "",
                
            };
            
            oxidTimeState3 = new LabelledEntryWidget
            {
                LabelText = "Time Oxid State1:",
                EntryText = "",
            };
            
            oxidPercentState3 = new LabelledEntryWidget
            {
                LabelText = "Position Oxid State1:",
                EntryText = "",
            };
            
            ignitionTime = new LabelledEntryWidget()
            {
                LabelText = "Ignition Time:",
                EntryText = ""
            };
            
        

            endTime = new LabelledEntryWidget
            {
                LabelText = "End Time:",
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
        
           // fuelState1.PackStart(headerFuelLabel1, false,false,0);
            fuelState1.PackStart(fuelTimeState1, false , false,0);
            fuelState1.PackEnd(fuelPercentState1, false ,false,0);
            //fuelState2.PackStart(headerFuelLabel2, false, false,0 );
            fuelState2.PackStart(fuelTimeState2,false,false,0);
            fuelState2.PackEnd(fuelPercentState2,false,false,0);
            //fuelState3.PackStart(headerFuelLabel3, false, false,0);
            fuelState3.PackStart(fuelTimeState3,false,false,0);
            fuelState3.PackEnd(fuelPercentState3, false,false,0);
            //oxidState1.PackStart(headerOxidLabel1, false, false, 0);
            oxidState1.PackStart(oxidTimeState1, false, false, 0);
            oxidState1.PackEnd(oxidPercentState1, false, false, 0);
            
            //oxidState2.PackStart(headerOxidLabel2, false, false, 0);
            oxidState2.PackStart(oxidTimeState2, false, false ,0);
            oxidState2.PackEnd(oxidPercentState2, false , false , 0);
            //oxidState3.PackStart(headerOxidLabel3, false, false, 0);
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

            String errorMsg = "";
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
            
            List<LabelledEntryWidget> entryValuesPosition = new List<LabelledEntryWidget>
            
            {
                fuelPercentState1,
                fuelPercentState2,
                fuelPercentState3,
                oxidPercentState1,
                oxidPercentState2,
                oxidPercentState3,
                
            };
            ushort[] intList = new ushort[15];
            int counter = 0;
            foreach (LabelledEntryWidget label in entryValues)
            {
                ushort.TryParse(label.EntryText, out intList[counter]);
               
            }

            
            for(int i = 0; i < entryValues.Count; i++)
            {                
                if (entryValues[i].EntryText.IsNullOrEmpty())
                {
                    errorMsg += "-" + entryValues[i].LabelText +" Must not be Empty\n";
                    
                }

            

            }
            if (Int32.Parse(fuelTimeState1.EntryText) > Int32.Parse(fuelTimeState2.EntryText))
            {
                errorMsg += fuelTimeState2.LabelText + " Must be higher than " + fuelTimeState1.LabelText + "\n";
            }
                
            if (Int32.Parse(fuelTimeState2.EntryText) > Int32.Parse(fuelTimeState3.EntryText))
            {
                errorMsg += fuelTimeState3.LabelText + " Must be higher than " + fuelTimeState2.LabelText + "\n";

            }
            
            if (Int32.Parse(oxidTimeState1.EntryText) > Int32.Parse(oxidTimeState2.EntryText))
            {
                errorMsg += oxidTimeState2.LabelText + " Must be higher than " + oxidTimeState1.LabelText + "\n";
            }
                
            if (Int32.Parse(oxidTimeState2.EntryText) > Int32.Parse(oxidTimeState3.EntryText))
            {
                errorMsg += oxidTimeState3.LabelText + " Must be higher than " + oxidTimeState2.LabelText + "\n";

            }

            int[] positionValues = new int[6];
            for (int i = 0; i < entryValuesPosition.Count; i++)
            {
                Int32.TryParse(entryValuesPosition[i].EntryText, out positionValues[i]);
                if (!Enumerable.Range(0,100).Contains(positionValues[i]))
                {
                    errorMsg +=  entryValuesPosition[i].LabelText + " Values must between 0 - 100 in \n";
                }
            }

            if (!errorMsg.IsNullOrEmpty())
            {
                MessageDialog errorDialog = new MessageDialog(this, 
                    DialogFlags.DestroyWithParent, 
                    MessageType.Error, 
                    ButtonsType.Close, 
                    errorMsg
                );
                errorDialog.Run();
                errorDialog.Destroy(); 

                
            }
            
        }
       
       
        
        
    }
  
    
    
}