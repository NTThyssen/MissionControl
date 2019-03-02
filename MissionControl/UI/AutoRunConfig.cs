using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Castle.Core.Internal;
using Gdk;
using Gtk;
using MissionControl.Connection.Commands;
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
        
        private LabelledEntryWidget _startDelay;
        private LabelledEntryWidget _ignitionTime;
        private LabelledEntryWidget _endTime;
        private LabelledEntryWidget _fuelTimeState1;
        private LabelledEntryWidget _fuelPercentState1;
        private LabelledEntryWidget _fuelTimeState2;
        private LabelledEntryWidget _fuelPercentState2;
        private LabelledEntryWidget _fuelTimeState3;
        private LabelledEntryWidget _fuelPercentState3;
        private LabelledEntryWidget _oxidTimeState1;
        private LabelledEntryWidget _oxidPercentState1;
        private LabelledEntryWidget _oxidTimeState2;
        private LabelledEntryWidget _oxidPercentState2;
        private LabelledEntryWidget _oxidTimeState3;
        private LabelledEntryWidget _oxidPercentState3;
   
        private Button _btnAutoRunConfigSave;
        
        public AutoRunConfig() : base(WindowType.Toplevel)
        {
            Title = "Auto Sequence Config";
            
            SetSizeRequest(950, 350);
            SetPosition(WindowPosition.Center);
            Console.WriteLine("hello autoconfig");
            
            
            _startDelay         = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "Start Delay:" };
            _ignitionTime       = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "Ignition Time:" };
            _endTime            = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "End Time:"};
            
            _fuelTimeState1     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 1 Time Fuel:" };
            _fuelPercentState1  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 1 Position Fuel:" };
            _oxidTimeState1     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 1 Time Oxid:" };
            _oxidPercentState1  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 1 Position Oxid:" };

            _fuelTimeState2     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 2 Time Fuel:" };
            _fuelPercentState2  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 2 Position Fuel:" };
            _oxidTimeState2     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 2 Time Oxid:" };
            _oxidPercentState2  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 2 Position Oxid:" };

            _fuelTimeState3     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 3 Time Fuel:" };
            _fuelPercentState3  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 3 Position Fuel:"};
            _oxidTimeState3     = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 3 Time Oxid:" };
            _oxidPercentState3  = new LabelledEntryWidget(0.0f, 0.5f) { LabelText = "State 3 Position Oxid:" };
            
            _btnAutoRunConfigSave = new Button{Label = "Save", WidthRequest = 80, HeightRequest = 40};
            
            /* Table How To ;)
             *
             *   0          1          2
             *  0+----------+----------+
             *   |          |          |
             *  1+----------+----------+
             *   |          |          |
             *  2+----------+----------+
             * 
             */
           
            Table layout = new Table(5, 4, true);
            layout.Attach(_startDelay, 0, 1, 0, 1);
            layout.Attach(_ignitionTime, 0, 1, 1, 2);
            layout.Attach(_endTime, 0, 1, 2, 3);
            
            layout.Attach(_fuelTimeState1, 1, 2, 0, 1);
            layout.Attach(_fuelPercentState1, 1, 2, 1, 2);
            layout.Attach(_oxidTimeState1, 1, 2, 2, 3);
            layout.Attach(_oxidPercentState1, 1, 2, 3, 4);
            
            layout.Attach(_fuelTimeState2, 2, 3, 0, 1);
            layout.Attach(_fuelPercentState2, 2, 3, 1, 2);
            layout.Attach(_oxidTimeState2, 2, 3, 2, 3);
            layout.Attach(_oxidPercentState2, 2, 3, 3, 4);
            
            layout.Attach(_fuelTimeState3, 3, 4, 0, 1);
            layout.Attach(_fuelPercentState3, 3, 4, 1, 2);
            layout.Attach(_oxidTimeState3, 3, 4, 2, 3);
            layout.Attach(_oxidPercentState3, 3, 4, 3, 4);
            
            layout.Attach(_btnAutoRunConfigSave, 3, 4, 4, 5, 0, 0, 20, 20);
            
            _btnAutoRunConfigSave.Pressed += getTimings;
            
            Add(layout);
            ShowAll();
        }
        
        public void getTimings(object sender, EventArgs e)
        {

            String errorMsg = "";
            bool error = false;
            AutoParameters ap = new AutoParameters();
            
            error |= ValidateTime(_startDelay, 0, ref errorMsg, out ap.startTime);
            error |= ValidateTime(_ignitionTime, ap.startTime, ref errorMsg, out ap.ignitionTime);
            
            error |= ValidateTime(_fuelTimeState1, ap.startTime, ref errorMsg, out ap.fuelState1Time);
            error |= ValidateTime(_oxidTimeState1, ap.startTime, ref errorMsg, out ap.oxidState1Time);
            
            error |= ValidateTime(_fuelTimeState2, ap.fuelState1Time, ref errorMsg, out ap.fuelState2Time);
            error |= ValidateTime(_oxidTimeState2, ap.oxidState1Time, ref errorMsg, out ap.oxidState2Time);
            
            error |= ValidateTime(_fuelTimeState3, ap.fuelState2Time, ref errorMsg, out ap.fuelState3Time);
            error |= ValidateTime(_oxidTimeState3, ap.oxidState2Time, ref errorMsg, out ap.oxidState3Time);

            error |= ValidateTime(_endTime, Math.Max(ap.fuelState3Time, ap.oxidState3Time), ref errorMsg, out ap.endTime);

            error |= ValidatePosition(_fuelPercentState1, ref errorMsg, out ap.fuelState1Percentage);
            error |= ValidatePosition(_oxidTimeState1, ref errorMsg, out ap.oxidState1Percentage);
            
            error |= ValidatePosition(_fuelPercentState2, ref errorMsg, out ap.fuelState2Percentage);
            error |= ValidatePosition(_oxidTimeState2, ref errorMsg, out ap.oxidState2Percentage);
            
            error |= ValidatePosition(_fuelPercentState3, ref errorMsg, out ap.fuelState3Percentage);
            error |= ValidatePosition(_oxidTimeState3, ref errorMsg, out ap.oxidState3Percentage);
            
            /*
           

            int[] positionValues = new int[6];
            for (int i = 0; i < entryValuesPosition.Count; i++)
            {
                Int32.TryParse(entryValuesPosition[i].EntryText, out positionValues[i]);
                
                       What? :D 
                if (!Enumerable.Range(0,100).Contains(positionValues[i]))
                {
                    errorMsg +=  entryValuesPosition[i].LabelText + " Values must between 0 - 100 in \n";
                }
            }*/

            if (error)
            {
                MessageDialog errorDialog = new MessageDialog(this, 
                    DialogFlags.DestroyWithParent, 
                    MessageType.Error, 
                    ButtonsType.Close, 
                    errorMsg
                );
                errorDialog.Run();
                errorDialog.Destroy();
                return;
            }
            
            
        }



        public bool ValidateTime(LabelledEntryWidget input, int lowerTime, ref string errMsg, out ushort time)
        {
            if (UInt16.TryParse(input.EntryText, out ushort result))
            {
                if (result >= lowerTime)
                {
                    time = result;
                    return false;
                }
                
                time = 0;
                errMsg += $"\"{input.LabelText}\" was smaller than required\n";
                return true;
            }

            time = 0;
            errMsg += $"\"{input.LabelText}\" was not an integer\n";
            return true;
        }

        public bool ValidatePosition(LabelledEntryWidget input, ref string errMsg, out float position)
        {
            if (float.TryParse(input.EntryText, out float result))
            {
                if (result >= 0.0 && result <= 100.0)
                {
                    position = result;
                    return false;
                }
                
                position = 0;
                errMsg += $"\"{input.LabelText}\" was not between 0.0 and 100.0\n";
                return true;
            }
            
            position = 0;
            errMsg += $"\"{input.LabelText}\" was not a floating point number\n";
            return true;
        }
       
       
        
        
    }
  
    
    
}