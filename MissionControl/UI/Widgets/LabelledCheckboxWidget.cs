using System;
using Gtk;

namespace MissionControl.UI
{
    public partial class LabelledCheckboxWidget : Gtk.Bin
    {
        private Label _label;
        private CheckButton _check;
        
        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }
        
        public bool Checked {
            get { return _check.Active; }
            set { _check.Active = value; }
        }

        public LabelledCheckboxWidget()
        {
            this.Build();
            
            _label = new Label();
            _check = new CheckButton();

            HBox box = new HBox(false, 0);
            box.PackStart(_label, false, false, 0);
            box.PackStart(_check, false, false, 0);

            Add(box);
        }
    }
}