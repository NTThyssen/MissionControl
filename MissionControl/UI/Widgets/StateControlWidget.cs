using System;
using System.Collections.Generic;
using Gtk;
using MissionControl.Connection.Commands;
using MissionControl.Data;

namespace MissionControl.UI.Widgets
{

    public interface IStateControlListener
    {
        void OnStatePressed(StateCommand command);
    }

    [System.ComponentModel.ToolboxItem(true)]
    public partial class StateControlWidget : Bin
    {

        IStateControlListener _listener;
        Dictionary<DToggleButton, StateCommand> _stateButtons;

        public StateControlWidget(List<StateCommand> states, IStateControlListener listener)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener), "A listener was not provided");

            Build();
            VBox container = new VBox(false, 10);

            DSectionTitle title = new DSectionTitle("States");
            container.PackStart(title, false, false, 0);

            _stateButtons = new Dictionary<DToggleButton, StateCommand>();

            for (int i = 0; i < states.Count; i++)
            {
                DToggleButton.ToggleState initialState = (i == 0) ? DToggleButton.ToggleState.Active : DToggleButton.ToggleState.Inactive;
                DToggleButton stateButton = new DToggleButton(80, 40, states[i].StateName, states[i].StateName, initialState);
                stateButton.Pressed += StateButton_Pressed;

                _stateButtons.Add(stateButton, states[i]);

                container.PackStart(stateButton, false, false, 0);
            }

            Add(container);

            ShowAll();

        }

        void StateButton_Pressed(object sender, EventArgs e)
        {
            _listener.OnStatePressed(_stateButtons[(DToggleButton)sender]);
            //((DToggleButton)sender).Set(DToggleButton.ToggleState.Intermediate);
        }


        public void SetCurrentState(StateCommand state)
        {
            foreach(KeyValuePair<DToggleButton, StateCommand> kv in _stateButtons)
            {
                if (kv.Value == state)
                {
                    kv.Key.Set(DToggleButton.ToggleState.Active);
                    kv.Key.Sensitive = false;
                }
                else
                {
                    kv.Key.Set(DToggleButton.ToggleState.Inactive);
                    kv.Key.Sensitive = true;
                }
            }
        }
    }
}
