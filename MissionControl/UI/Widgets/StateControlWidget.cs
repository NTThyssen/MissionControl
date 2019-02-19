using System;
using System.Collections.Generic;
using Gtk;
using MissionControl.Connection.Commands;
using MissionControl.Data;

namespace MissionControl.UI.Widgets
{

    public interface IStateControlListener
    {
        void OnStatePressed(State command);
    }

    [System.ComponentModel.ToolboxItem(true)]
    public partial class StateControlWidget : Bin
    {

        IStateControlListener _listener;
        Dictionary<Button, State> _stateButtons;

        public StateControlWidget(List<State> states, IStateControlListener listener)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener), "A listener was not provided");

            Build();
            VBox container = new VBox(false, 10);

            _stateButtons = new Dictionary<Button, State>();

            for (int i = 0; i < states.Count; i++)
            {
                Button stateButton = new Button
                {
                    Label = states[i].StateName,
                    WidthRequest = 80,
                    HeightRequest = 40
                };
                stateButton.Pressed += StateButton_Pressed;


                _stateButtons.Add(stateButton, states[i]);

                container.PackStart(stateButton, false, false, 0);
            }

            Add(container);

            ShowAll();

        }

        void StateButton_Pressed(object sender, EventArgs e)
        {
            _listener.OnStatePressed(_stateButtons[(Button)sender]);
        }


        public void SetCurrentState(State state, bool isAuto)
        {
            foreach(KeyValuePair<Button, State> kv in _stateButtons)
            {

                if (isAuto)
                {
                    if (kv.Value.StateID == state.StateID)
                    {
                        kv.Key.ModifyBg(StateType.Normal, Colors.ButtonHighlight);
                        kv.Key.ModifyBg(StateType.Prelight, Colors.ButtonHighlight);
                        kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonHighlight);
                    }
                    else
                    {
                        kv.Key.ModifyBg(StateType.Normal, Colors.ButtonDim);
                        kv.Key.ModifyBg(StateType.Prelight, Colors.ButtonDim);
                        kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonDisabled);
                    }
                    kv.Key.Sensitive = false;
                }
                else
                {
                    if (kv.Value.StateID == state.StateID)
                    {
                        kv.Key.ModifyBg(StateType.Normal, Colors.ButtonHighlight);
                        kv.Key.ModifyBg(StateType.Prelight, Colors.ButtonHighlight);
                        kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonHighlight);
                        kv.Key.Sensitive = false;
                    }
                    else
                    {
                        kv.Key.ModifyBg(StateType.Normal, Colors.ButtonNormal);
                        kv.Key.ModifyBg(StateType.Prelight, Colors.ButtonNormal);
                        kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonDisabled);
                        kv.Key.Sensitive = true;
                    }
                }
            }
        }


    }
}
