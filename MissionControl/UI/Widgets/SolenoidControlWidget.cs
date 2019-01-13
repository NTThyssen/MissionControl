using System;
using Gtk;
using Gdk;
using MissionControl.Data.Components;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SolenoidControlWidget : Bin
    {

        public delegate void SolenoidCallback(SolenoidComponent component, bool active);

        private readonly string OPEN = "Open";
        private readonly string CLOSED = "Closed";

        private VBox _container;
        private Label _name;
        private ToggleButton _toggle;

        private SolenoidCallback _callback;
        private SolenoidComponent _component;

        public SolenoidControlWidget(SolenoidComponent component, SolenoidCallback callback)
        {
            this.Build();
            _callback = callback;
            _component = component;

            _container = new VBox(false, 5);
            _name = new Label
            {
                Text = component.Name,
                Xalign = 0
            };
            _name.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

            _toggle = new ToggleButton
            {
                WidthRequest = 40,
                HeightRequest = 40
            };
            _toggle.ModifyBg(StateType.Active, new Gdk.Color(57, 120, 77));

            _toggle.Toggled += OnToggleChanged;
            SetToggleText(component.Open);

            _container.PackStart(_name, false, false, 0);
            _container.PackStart(_toggle, false, false, 0);

            Add(_container);
        }

        private void OnToggleChanged(object sender, EventArgs e)
        {
            SetToggleText(_toggle.Active);
            _callback(_component, _toggle.Active);

        }

        private void SetToggleText(bool active)
        {
 
            _toggle.Label = (active) ? OPEN : CLOSED;
            Color c = (active) ? new Gdk.Color(70, 148, 95) : new Gdk.Color(230, 230, 230);
            _toggle.ModifyBg(StateType.Prelight, c);

        }

    }
}
