using System;
using Gtk;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class LabelledEntryWidget : Bin
    {

        private HBox _box;
        private Label _label;
        private Entry _text;

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public string EntryText
        {
            get { return _text.Text; }
            set { _text.Text = value; }
        }

        public LabelledEntryWidget()
        {
            this.Build();

            _box = new HBox(false, 20);
            _label = new Label();
            _text = new Entry
            {
                WidthRequest = 60
            };

            _box.PackStart(_label, true, false, 0);
            _box.PackStart(_text, false, false, 30);

            Add(_box);
        }

        public LabelledEntryWidget(string label) : this()
        {
            _label.Text = label;
        }

        public LabelledEntryWidget(string label, string text) : this(label)
        {
            _text.Text = text;
        }
    }
}
