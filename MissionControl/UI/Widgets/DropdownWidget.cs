using System;
using System.Collections.Generic;
using Gtk;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DropdownWidget : Gtk.Bin
    {
        private ComboBox _dropdown;
        private Label _label;
        private ListStore _store;
        private List<string> _values;

        public int Active { get { return _dropdown.Active; } }
        public string ActiveText { get { return _dropdown.ActiveText; } }

        public DropdownWidget(string label)
        {
            this.Build();

            HBox box = new HBox(false, 10);
            _store = new ListStore(typeof(string));
            _values = new List<string>();

            CellRendererText cell = new CellRendererText();

            _dropdown = new ComboBox();
            _dropdown.PackStart(cell, false);
            _dropdown.AddAttribute(cell, "text", 0);
            _dropdown.Model = _store;

            if (!string.IsNullOrEmpty(label))
            {
                _label = new Label(label);
                box.PackStart(_label, false, false, 0);
            }

            box.PackStart(_dropdown, false, false, 0);

            Add(box);
        }

        public DropdownWidget(string label, string[] values) : this(label)
        {
            AddOnlyAll(values);
        }

        public void Clear()
        {
            _store.Clear();
            _values.Clear();
        }

        public void AddAll(string[] values)
        {
            foreach (string value in values)
            {
                _values.Add(value);
                _store.AppendValues(value);
            }
        }

        public void AddOnlyAll(string[] values)
        {
            Clear();
            AddAll(values);
        }

        public void Set(string value)
        {
            int index = _values.IndexOf(value);
            if (index != -1)
            {
                _dropdown.Active = index;
            }
        }
    }
}
