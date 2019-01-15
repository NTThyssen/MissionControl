using System;
using Gtk;
using Gdk;
using MissionControl.Data.Components;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SolenoidControlWidget : EventBox
    {

        public delegate void SolenoidCallback(SolenoidComponent component, bool active);

        private readonly string OPEN = "Open";
        private readonly string CLOSED = "Closed";

        private SolenoidCallback _callback;
        private SolenoidComponent _component;

        private DToggleButton _toggle;

        public SolenoidControlWidget(SolenoidComponent component, SolenoidCallback callback)
        {
            this.Build();

            VisibleWindow = false;
            AboveChild = false;

            _callback = callback;
            _component = component;

            VBox container = new VBox(false, 5);
            Label name = new Label
            {
                Text = component.Name,
                Xalign = 0
            };
            name.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

            _toggle = new DToggleButton(40, 40, OPEN, CLOSED, DToggleButton.ToggleState.Inactive);
            _toggle.Pressed += Toggle_Pressed;

            container.PackStart(name, false, false, 0);
            container.PackStart(_toggle, true, true, 0);

            Add(container);
            ShowAll();
        }

        void Toggle_Pressed(object sender, EventArgs e)
        {
            _toggle.Toggle();
            _callback(_component, _toggle.Active);
        }

    }
}
