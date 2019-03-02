using System;
using Gtk;

namespace MissionControl.UI
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class LabelledRadioWidget : Gtk.Bin
    {
        private Label _label;
        private RadioButton _absolute;
        private RadioButton _relative;

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public bool ShowAbsolutePressure {
            get { return _absolute.Active; } 
            set { if (value) { _absolute.Active = true; } else { _relative.Active = true; } }
        }

        public LabelledRadioWidget()
        {
            this.Build();

            _label = new Label();
            _absolute = new RadioButton(null, "Absolute");
            _relative = new RadioButton(_absolute, "Relative");

            HBox box = new HBox(false, 0);
            box.PackStart(_label, false, false, 30);
            box.PackStart(_absolute, false, false, 0);
            box.PackStart(_relative, false, false, 0);

            Add(box);
        }
    }
}
