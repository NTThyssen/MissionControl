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

        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; }
        }

        public LabelledEntryWidget(string label)
        {
            this.Build();

            _box = new HBox(false, 20);
            _label = new Label
            {
                Text = label
            };
            _text = new Entry
            {
                WidthRequest = 60
            };

            _box.PackStart(_label, true, false, 0);
            _box.PackStart(_text, false, false, 60);

            Add(_box);
        }
    }
}
