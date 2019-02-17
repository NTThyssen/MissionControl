using System;
using Gtk;
using MissionControl.Data.Components;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ComponentSettingWidget : Gtk.Bin
    {

        Entry _min, _max;
        CheckButton _enable;

        public bool Enabled
        {
            get { return _enable.Active; }
            set { _enable.Active = value; }
        }

        public string Min
        {
            get { return _min.Text; }
            set { _min.Text = value; }
        }

        public string Max
        {
            get { return _max.Text; }
            set { _max.Text = value; }
        }

        public Component Component { get; }

        public ComponentSettingWidget(Component component)
        {
            this.Build();

            Component = component;

            VBox vertical = new VBox(false, 0);
            HBox top = new HBox(false, 0);
            HBox bottom = new HBox(false, 0);

            Label lblMin, lblMax, lblName;
            lblMin = new Label("Min:");
            lblMax = new Label("Max:");
            lblName = new Label(component.Name);

            _min = new Entry {
                WidthRequest = 40
            };

            _max = new Entry { 
                WidthRequest = 40
            };

            _enable = new CheckButton { 
                Active = component.Enabled,
                Label = "Enable sensor" 
            };

            top.PackStart(lblName, false, false, 0);
            top.PackStart(_enable, true, false, 0);

            if (component is IWarningLimits warn)
            {
                _min.Text = (!float.IsNaN(warn.MinLimit)) ? warn.MinLimit.ToString() : string.Empty;
                _max.Text = (!float.IsNaN(warn.MaxLimit)) ? warn.MaxLimit.ToString() : string.Empty;
                bottom.PackStart(lblMin, false, false, 0);
                bottom.PackStart(_min, false, false, 10);
                bottom.PackStart(lblMax, false, false, 0);
                bottom.PackStart(_max, false, false, 10);
            }
         
            vertical.PackStart(top, false, false, 0);
            vertical.PackStart(bottom, false, false, 0);

            Add(vertical);

        }
    }
}
