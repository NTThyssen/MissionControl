using System;
using System.Collections.Generic;
using Gtk;
using MissionControl.Connection.Commands;
using MissionControl.Data;

namespace MissionControl.UI.Widgets
{

    [System.ComponentModel.ToolboxItem(true)]
    public partial class StateControlWidget : Bin
    {
        Dictionary<EventBox, State> _stateTexts;

        public StateControlWidget(List<State> states)
        {

            Build();
            VBox container = new VBox(false, 5);

            _stateTexts = new Dictionary<EventBox, State>();

            for (int i = 0; i < states.Count; i++)
            {
                EventBox backgroundBox = new EventBox()
                {
                    WidthRequest = 80,
                    HeightRequest = 25
                };
                
                Label text = new Label
                {
                    Text = states[i].StateName
                };

                backgroundBox.Add(text);
                _stateTexts.Add(backgroundBox, states[i]);

                container.PackStart(backgroundBox, false, false, 0);
            }

            Add(container);

            ShowAll();

        }

        public void SetCurrentState(State state, bool isAuto)
        {
            foreach(KeyValuePair<EventBox, State> kv in _stateTexts)
            {
                if (kv.Value.StateID == state.StateID)
                {
                    kv.Key.ModifyBg(StateType.Normal, Colors.ButtonHighlight);
                    kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonHighlight);
                    kv.Key.Children[0].ModifyFg(StateType.Normal, Colors.ButtonHighlightText);
                    kv.Key.Children[0].ModifyFg(StateType.Insensitive, Colors.ButtonHighlightText);
                }
                else
                {
                    kv.Key.ModifyBg(StateType.Normal, Colors.ButtonNormal);
                    kv.Key.ModifyBg(StateType.Insensitive, Colors.ButtonNormal);
                    kv.Key.Children[0].ModifyFg(StateType.Normal, Colors.ButtonNormalText);
                    kv.Key.Children[0].ModifyFg(StateType.Insensitive, Colors.ButtonNormalText);
                }
            }
        }


    }
}
