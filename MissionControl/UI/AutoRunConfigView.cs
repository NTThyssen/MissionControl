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

    public interface IAutoParameterListener
    {
        void OnParametersSave(AutoParameters ap);
    }
    
    public class AutoRunConfigView : Window
    {
        private IAutoParameterListener _listener;
        
        private LabelledEntryWidget _startDelay;
        private LabelledEntryWidget _ignitionTime;
        private LabelledEntryWidget _preStage1Time;
        private LabelledEntryWidget _preStage2MaxTime;
        private LabelledEntryWidget _preStage2StableTime;
        private LabelledEntryWidget _rampUpStableTime;
        private LabelledEntryWidget _rampUpMaxTime;
        private LabelledEntryWidget _burnTime;
        private LabelledEntryWidget _shutdown1Time;
        private LabelledEntryWidget _shutdown2Time;
        private LabelledEntryWidget _flushTime;

        private LabelledEntryWidget _preStage1FuelPosition;
        private LabelledEntryWidget _preStage2FuelPosition;
        private LabelledEntryWidget _rampUpFuelPosition;
        private LabelledEntryWidget _shutdown1FuelPosition;
        private LabelledEntryWidget _shutdown2FuelPosition;
        
        private LabelledEntryWidget _preStage1OxidPosition;
        private LabelledEntryWidget _preStage2OxidPosition;
        private LabelledEntryWidget _rampUpOxidPosition;
        private LabelledEntryWidget _shutdown1OxidPosition;
        private LabelledEntryWidget _shutdown2OxidPosition;

        private LabelledEntryWidget _preStage2StablePressure;
        private LabelledEntryWidget _chamberPressurePressure;
        private LabelledEntryWidget _emptyFuelFeedPressureThreshold;
        private LabelledEntryWidget _emtpyOxidFeedPressureThreshold;
        
        private Button _btnAutoRunConfigSave;
        
        public AutoRunConfigView(IAutoParameterListener listener, AutoParameters ap) : base(WindowType.Toplevel)
        {
            _listener = listener;
            Title = "Auto Sequence Config";
            
            //SetSizeRequest(950, 350);
            SetPosition(WindowPosition.Center);

            _startDelay = CreateWidget("Start Delay [ms]", GetValueString(ap.StartDelay));
            _ignitionTime = CreateWidget("Ignition Time [ms]", GetValueString(ap.IgnitionTime));
            _preStage1Time = CreateWidget("Pre Stage 1 Time [ms]", GetValueString(ap.PreStage1Time));
            _preStage2MaxTime = CreateWidget("Pre Stage 2 Max Time [ms]", GetValueString(ap.PreStage2MaxTime));
            _preStage2StableTime = CreateWidget("Pre Stage 2 Stable Time [ms]", GetValueString(ap.PreStage2StableTime));
            _rampUpStableTime = CreateWidget("Ramp Up Stable Time [ms]", GetValueString(ap.RampUpStableTime));
            _rampUpMaxTime = CreateWidget("Ramp Up Max Time [ms]", GetValueString(ap.RampUpMaxTime));
            _burnTime = CreateWidget("Burn Time [ms]", GetValueString(ap.BurnTime));
            _shutdown1Time = CreateWidget("Shutdown 1 Time [ms]", GetValueString(ap.Shutdown1Time));
            _shutdown2Time = CreateWidget("Shutdown 2 Time [ms]", GetValueString(ap.Shutdown2Time));
            _flushTime = CreateWidget("Flush Time [ms]", GetValueString(ap.FlushTime));
            
            _preStage1FuelPosition = CreateWidget("Pre Stage 1 Fuel Pos. [%]", GetValueString(ap.PreStage1FuelPosition));
            _preStage2FuelPosition = CreateWidget("Pre Stage 2 Fuel Pos. [%]", GetValueString(ap.PreStage2FuelPosition));
            _rampUpFuelPosition = CreateWidget("Ramp Up Fuel Pos. [%]", GetValueString(ap.RampUpFuelPosition));
            _shutdown1FuelPosition = CreateWidget("Shutdown 1 Fuel Pos. [%]", GetValueString(ap.Shutdown1FuelPosition));
            _shutdown2FuelPosition = CreateWidget("Shutdown 2 Fuel Pos. [%]", GetValueString(ap.Shutdown2FuelPosition));
            
            _preStage1OxidPosition = CreateWidget("Pre Stage 1 Oxid Pos. [%]", GetValueString(ap.PreStage1OxidPosition));
            _preStage2OxidPosition = CreateWidget("Pre Stage 2 Oxid Pos. [%]", GetValueString(ap.PreStage2OxidPosition));
            _rampUpOxidPosition = CreateWidget("Ramp Up Oxid Pos. [%]", GetValueString(ap.RampUpOxidPosition));
            _shutdown1OxidPosition = CreateWidget("Shutdown 1 Oxid Pos. [%]", GetValueString(ap.Shutdown1OxidPosition));
            _shutdown2OxidPosition = CreateWidget("Shutdown 2 Oxid Pos. [%]", GetValueString(ap.Shutdown2OxidPosition));
            
            _preStage2StablePressure = CreateWidget("Pre Stage 2 Stable Pres. [bara]", GetValueString(ap.PreStage2StablePressure));
            _chamberPressurePressure = CreateWidget("Cham. Pres. Thres. [bara]", GetValueString(ap.ChamberPressurePressure));
            _emptyFuelFeedPressureThreshold = CreateWidget("Empty Fuel Pres. [bara]", GetValueString(ap.EmtpyFuelFeedPressureThreshold));
            _emtpyOxidFeedPressureThreshold = CreateWidget("Empty Oxid Pres. [bara]", GetValueString(ap.EmtpyOxidFeedPressureThreshold));
            
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
           
            Table layout = new Table(6, 7, false);
            
            AddToTable(layout, _startDelay, 0, 1, 0, 1);
            AddToTable(layout, _ignitionTime, 0, 1, 1, 2);
            AddToTable(layout, _preStage1Time, 0, 1, 2, 3);
            AddToTable(layout, _preStage1FuelPosition, 0, 1, 3, 4);
            AddToTable(layout, _preStage1OxidPosition, 0, 1, 4, 5);
            
            AddToTable(layout, _preStage2MaxTime, 1, 2, 0, 1);
            AddToTable(layout, _preStage2StableTime, 1, 2, 1, 2);
            AddToTable(layout, _preStage2FuelPosition, 1, 2, 2, 3);
            AddToTable(layout, _preStage2OxidPosition, 1, 2, 3, 4);
            AddToTable(layout, _preStage2StablePressure, 1, 2, 4, 5);
            
            AddToTable(layout, _rampUpMaxTime, 2, 3, 0, 1);
            AddToTable(layout, _rampUpStableTime, 2, 3, 1, 2);
            AddToTable(layout, _rampUpFuelPosition, 2, 3, 2, 3);
            AddToTable(layout, _rampUpOxidPosition, 2, 3, 3, 4);
            
            AddToTable(layout, _burnTime, 3, 4, 0, 1);
            AddToTable(layout, _chamberPressurePressure, 3, 4, 1, 2);
            AddToTable(layout, _emptyFuelFeedPressureThreshold, 3, 4, 2, 3);
            AddToTable(layout, _emtpyOxidFeedPressureThreshold, 3, 4, 3, 4);
            
            AddToTable(layout, _shutdown1Time, 4, 5, 0,1);
            AddToTable(layout, _shutdown1FuelPosition, 4, 5, 1, 2);
            AddToTable(layout, _shutdown1OxidPosition, 4, 5, 2, 3);
            
            AddToTable(layout, _shutdown2Time, 5, 6, 0,1);
            AddToTable(layout, _shutdown2FuelPosition, 5, 6, 1, 2);
            AddToTable(layout, _shutdown2OxidPosition, 5, 6, 2, 3);
            
            AddToTable(layout, _flushTime, 6, 7, 0, 1);
            
            layout.Attach(_btnAutoRunConfigSave, 6, 7, 5, 6, 0, 0, 20, 20);
            
            _btnAutoRunConfigSave.Pressed += getTimings;
            
            Add(layout);
            ShowAll();
        }

        private LabelledEntryWidget CreateWidget(string label, string value)
        {
            return new LabelledEntryWidget(0.0f, 0.0f) {  LabelText = label , EntryText = value, };
        }

        private void AddToTable(Table table, LabelledEntryWidget w, uint l, uint r, uint t, uint b)
        {
            table.Attach(w, l, r, t, b, 0, 0, 4, 20);
        }
        
        public void getTimings(object sender, EventArgs e)
        {

            String errorMsg = "";
            bool error = false;
            AutoParameters ap = new AutoParameters();
            
            error |= AutoParameters.ValidateTime(_startDelay.EntryText,nameof(ap.StartDelay), ref errorMsg, out ap.StartDelay);
            error |= AutoParameters.ValidateTime(_ignitionTime.EntryText,nameof(ap.PreStage1Time), ref errorMsg, out ap.IgnitionTime);
            error |= AutoParameters.ValidateTime(_preStage1Time.EntryText,nameof(ap.PreStage1Time), ref errorMsg, out ap.PreStage1Time);
            error |= AutoParameters.ValidateTime(_preStage2MaxTime.EntryText,nameof(ap.PreStage2MaxTime), ref errorMsg, out ap.PreStage2MaxTime);
            error |= AutoParameters.ValidateTime(_preStage2StableTime.EntryText,nameof(ap.PreStage2StableTime), ref errorMsg, out ap.PreStage2StableTime);
            error |= AutoParameters.ValidateTime(_rampUpStableTime.EntryText,nameof(ap.RampUpStableTime), ref errorMsg, out ap.RampUpStableTime);
            error |= AutoParameters.ValidateTime(_rampUpMaxTime.EntryText,nameof(ap.RampUpMaxTime), ref errorMsg, out ap.RampUpMaxTime);
            error |= AutoParameters.ValidateTime(_burnTime.EntryText,nameof(ap.BurnTime), ref errorMsg, out ap.BurnTime);
            error |= AutoParameters.ValidateTime(_shutdown1Time.EntryText,nameof(ap.Shutdown1Time), ref errorMsg, out ap.Shutdown1Time);
            error |= AutoParameters.ValidateTime(_shutdown2Time.EntryText,nameof(ap.Shutdown2Time), ref errorMsg, out ap.Shutdown2Time);
            error |= AutoParameters.ValidateTime(_flushTime.EntryText,nameof(ap.FlushTime), ref errorMsg, out ap.FlushTime);
            
            error |= AutoParameters.ValidatePosition(_preStage1FuelPosition.EntryText,nameof(ap.PreStage1FuelPosition), ref errorMsg, out ap.PreStage1FuelPosition);
            error |= AutoParameters.ValidatePosition(_preStage2FuelPosition.EntryText,nameof(ap.PreStage2FuelPosition), ref errorMsg, out ap.PreStage2FuelPosition);
            error |= AutoParameters.ValidatePosition(_rampUpFuelPosition.EntryText,nameof(ap.RampUpFuelPosition), ref errorMsg, out ap.RampUpFuelPosition);
            error |= AutoParameters.ValidatePosition(_shutdown1FuelPosition.EntryText,nameof(ap.Shutdown1FuelPosition), ref errorMsg, out ap.Shutdown1FuelPosition);
            error |= AutoParameters.ValidatePosition(_shutdown2FuelPosition.EntryText,nameof(ap.Shutdown2FuelPosition), ref errorMsg, out ap.Shutdown2FuelPosition);
            
            error |= AutoParameters.ValidatePosition(_preStage1OxidPosition.EntryText,nameof(ap.PreStage1OxidPosition), ref errorMsg, out ap.PreStage1OxidPosition);
            error |= AutoParameters.ValidatePosition(_preStage2OxidPosition.EntryText,nameof(ap.PreStage2OxidPosition), ref errorMsg, out ap.PreStage2OxidPosition);
            error |= AutoParameters.ValidatePosition(_rampUpOxidPosition.EntryText,nameof(ap.RampUpOxidPosition), ref errorMsg, out ap.RampUpOxidPosition);
            error |= AutoParameters.ValidatePosition(_shutdown1OxidPosition.EntryText,nameof(ap.Shutdown1OxidPosition), ref errorMsg, out ap.Shutdown1OxidPosition);
            error |= AutoParameters.ValidatePosition(_shutdown2OxidPosition.EntryText,nameof(ap.Shutdown2OxidPosition), ref errorMsg, out ap.Shutdown2OxidPosition);

            error |= AutoParameters.ValidatePressure(_preStage2StablePressure.EntryText,nameof(ap.PreStage2StablePressure), ref errorMsg, out ap.PreStage2StablePressure);
            error |= AutoParameters.ValidatePressure(_chamberPressurePressure.EntryText,nameof(ap.ChamberPressurePressure), ref errorMsg, out ap.ChamberPressurePressure);
            error |= AutoParameters.ValidatePressure(_emptyFuelFeedPressureThreshold.EntryText,nameof(ap.EmtpyFuelFeedPressureThreshold), ref errorMsg, out ap.EmtpyFuelFeedPressureThreshold);
            error |= AutoParameters.ValidatePressure(_emtpyOxidFeedPressureThreshold.EntryText,nameof(ap.EmtpyOxidFeedPressureThreshold), ref errorMsg, out ap.EmtpyOxidFeedPressureThreshold);
        

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
                
            }
            else
            {
                _listener.OnParametersSave(ap);    
            }
        }

        private string GetValueString(ushort value)
        {
            return (value != 0) ? value.ToString() : "";
        }
        
        private string GetValueString(float value)
        {
            return (value != 0) ? value.ToString(CultureInfo.InvariantCulture) : "";
        }   
    }
}