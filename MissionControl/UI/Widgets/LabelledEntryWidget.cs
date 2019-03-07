using System;
using Gtk;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class LabelledEntryWidget : Bin
    {

        private Label _label;
        private Entry _text;

        public string LabelText
        {
            get
            {
                return _label.Text; 
                   
            }
            set { _label.Text = value; }
        }

        public string EntryText
        {
            get { return _text.Text; }
            set { _text.Text = value; }
            
        }

        public LabelledEntryWidget(bool stacked = false)
        {
            this.Build();
            Box box;
            _label = new Label();
            _text = new Entry();
            
            if (stacked)
            {
                
                box = new VBox(false, 5);
                box.PackStart(_label, true, false, 0);
                box.PackStart(_text, false, false, 0);
            }
            else
            {
                
                box = new HBox(false, 20);
                box.PackStart(_label, true, false, 30);
                box.PackEnd(_text, false, false, 30);
            }
            
            Add(box);
        }

        public LabelledEntryWidget(string label) : this()
        {
            _label.Text = label;
        }
        
        public LabelledEntryWidget(float xalign, float yalign) : this(true)
        {
            _label.SetAlignment(xalign, yalign);
            
        }

        public LabelledEntryWidget(string label, string text) : this(label)
        {
            _text.Text = text;
        }
    }
}
