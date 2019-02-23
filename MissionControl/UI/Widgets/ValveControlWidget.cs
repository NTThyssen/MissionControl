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
        void OnSolenoidPressed(SolenoidComponent solenoid, bool active);
        void OnControlEnter(ValveComponent component);
        void OnControlLeave(ValveComponent component);
    }

    [System.ComponentModel.ToolboxItem(true)]
    public partial class ValveControlWidget : Gtk.Bin
    {
        List<Widget> _widgets;

        public ValveControlWidget(List<Component> components, IValveControlListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener), "A listener was not provided");

            Build();

            VBox controls = new VBox(false, 15);

            foreach (Component c in components)
            {
                if (c is ServoComponent servo)
                {
                    ServoControlWidget servoWidget = new ServoControlWidget(servo, listener.OnServoPressed);

                    servoWidget.EnterNotifyEvent += (o, args) => { listener.OnControlEnter(servo); args.RetVal = false; };
                    servoWidget.LeaveNotifyEvent += (o, args) => { listener.OnControlLeave(servo); args.RetVal = false; };

                    controls.PackStart(servoWidget, false, false, 0);
                } 
                else if (c is SolenoidComponent solenoid)
                {
                    SolenoidControlWidget solWidget = new SolenoidControlWidget(solenoid, listener.OnSolenoidPressed);

                    solWidget.EnterNotifyEvent += (o, args) => { listener.OnControlEnter(solenoid); args.RetVal = false; };
                    solWidget.LeaveNotifyEvent += (o, args) => { listener.OnControlLeave(solenoid); args.RetVal = false; };

                    controls.PackStart(solWidget, false, false, 0);
                }
            }

            Add(controls);

            ShowAll();
        }

        public void Sen ()
        {
            foreach(Widget w in _widgets)
            {
                w.Sensitive = false;
            }
        }

        public void DisableAll ()
        {

        }



    }
}
