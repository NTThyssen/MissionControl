using System;
using Gtk;
using MissionControl.Data.Components;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ServoControlWidget : Gtk.Bin
    {
        public delegate void ServoCallback(ServoComponent component, float value); 

        private VBox _container;
        private HBox _controls;
        private Label _name;
        private Button _openButton, _closeButton, _setButton;
        private Entry _setInput;

        private ServoCallback _callback;
        private ServoComponent _component;

        public ServoControlWidget(ServoComponent component, ServoCallback callback)
        {
            _component = component;
            _callback = callback;

            Build();
            _controls = new HBox(false, 5); // Maybe set homogenous

            _openButton = new Button
            {
                Label = "Open",
                HeightRequest = 40
            };

            _closeButton = new Button
            {
                Label = "Close",
                HeightRequest = 40
            };

            _setButton = new Button
            {
                Label = "Set",
                HeightRequest = 15
            };

            _setInput = new Entry {
                WidthChars = 5,
                HeightRequest = 40,

            };

            _controls.PackStart(_openButton, false, false, 0);
            _controls.PackStart(_closeButton, false, false, 0);
            _controls.PackStart(_setInput, false, false, 0);
            _controls.PackStart(_setButton, false, false, 0);

            _container = new VBox(false, 5);

            _name = new Label()
            {
                Text = _component.Name,
                Xalign = 0.0f,
                Yalign = 0.0f
            };
            _name.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

            _container.PackStart(_name, false, false, 0);
            _container.PackStart(_controls, false, false, 0);

            Add(_container);

        }
    }
}
