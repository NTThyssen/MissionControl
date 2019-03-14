using System;
using System.Collections.Generic;
using Gtk;
using MissionControl.Data.Components;
using Pango;

namespace MissionControl.UI.Widgets
{

    public interface IValveControlListener
    {
        void OnServoPressed(ServoComponent servo, float value);
        void OnServoTimed(ServoComponent servo, float startValue, float endValue, int delayMillis);
        void OnSolenoidPressed(SolenoidComponent solenoid, bool active);
        void OnControlEnter(ValveComponent component);
        void OnControlLeave(ValveComponent component);
    }

    [System.ComponentModel.ToolboxItem(true)]
    public partial class ValveControlWidget : Gtk.Bin
    {
        private List<ServoControlWidget> _servos;
        private List<SolenoidControlWidget> _solenoids;

        public ValveControlWidget(List<Component> components, IValveControlListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener), "A listener was not provided");

            Build();

            VBox controls = new VBox(false, 15);
            _servos = new List<ServoControlWidget>();
            _solenoids = new List<SolenoidControlWidget>();

            foreach (Component c in components)
            {
                if (c is ServoComponent servo)
                {
                    ServoControlWidget servoWidget = new ServoControlWidget(servo, listener.OnServoPressed, listener.OnServoTimed);

                    servoWidget.EnterNotifyEvent += (o, args) => { listener.OnControlEnter(servo); args.RetVal = false; };
                    servoWidget.LeaveNotifyEvent += (o, args) => { listener.OnControlLeave(servo); args.RetVal = false; };

                    _servos.Add(servoWidget);
                } 
                else if (c is SolenoidComponent solenoid)
                {
                    SolenoidControlWidget solWidget = new SolenoidControlWidget(solenoid, listener.OnSolenoidPressed);

                    solWidget.EnterNotifyEvent += (o, args) => { listener.OnControlEnter(solenoid); args.RetVal = false; };
                    solWidget.LeaveNotifyEvent += (o, args) => { listener.OnControlLeave(solenoid); args.RetVal = false; };

                    _solenoids.Add(solWidget);
                }
            }
            
            List<HBox> solPairs = new List<HBox>();
            for (int i = 0; i < _solenoids.Count; i++)
            {
                if (i % 2 == 0)
                {
                    HBox box = new HBox(true,15);
                    box.PackStart(_solenoids[i]);
                    solPairs.Add(box);
                }
                else
                {
                    solPairs[solPairs.Count - 1].PackStart(_solenoids[i]);
                }
            }
            
            foreach (HBox solPair in solPairs)
            {
                controls.PackStart(solPair, false, false, 0);
            }

            foreach (ServoControlWidget servoWidget in _servos)
            {
                controls.PackStart(servoWidget, false, false, 0);
            }

            Add(controls);

            ShowAll();
        }

        public void UpdateControls(Dictionary<byte, Component> components)
        {
            foreach (SolenoidControlWidget solenoid in _solenoids)
            {
                byte id = solenoid.ComponentID;
                if (components.ContainsKey(id) && components[id] is SolenoidComponent sc)
                {
                    solenoid.Set(sc.Open);   
                }
            }
        } 
    }
}
