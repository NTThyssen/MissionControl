using System;
using System.Collections.Generic;
using Gtk;
using MissionControl.Data.Components;
using Pango;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ValveControlWidget : Gtk.Bin
    {
        public ValveControlWidget(List<Component> components, 
            ServoControlWidget.ServoCallback servoCallback, 
            SolenoidControlWidget.SolenoidCallback solenoidCallback)
        {
            Build();
            VBox controls = new VBox(false, 15);

            Label title = new Label
            {
                Text = "Valves",
                Xalign = 0
            };

            title.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
            title.ModifyFont(new FontDescription
            {
                Size = Convert.ToInt32(24 * Pango.Scale.PangoScale)
            });

            controls.PackStart(title, false, false, 0);

            foreach (Component c in components)
            {
                if (c is ServoComponent ser)
                {
                    ServoControlWidget servo = new ServoControlWidget(ser, servoCallback);
                    controls.PackStart(servo, false, false, 0);
                } 
                else if (c is SolenoidComponent sol)
                {
                    SolenoidControlWidget solenoid = new SolenoidControlWidget(sol, solenoidCallback);
                    controls.PackStart(solenoid, false, false, 0);
                }
            }

            Add(controls);

            ShowAll();
        }
    }
}
