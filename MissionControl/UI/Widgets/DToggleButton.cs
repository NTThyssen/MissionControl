﻿using System;
using Gdk;
using Gtk;

namespace MissionControl.UI.Widgets
{
    public class DToggleButton : Button
    {
        private readonly string _activeText;
        private readonly string _inactiveText;

        // Styling
        Color _activeColor = new Color(57, 120, 77);
        Color _activePrelight = new Color(70, 148, 95);
        Color _inactiveColor = new Color(220, 220, 220);
        Color _inactivePrelight = new Color(230, 230, 230);

        public enum ToggleState
        {
            Inactive,
            Active,
            Intermediate
        }

        ToggleState _state;
        public delegate void ToggleCallback(ToggleState newState);

        public DToggleButton(int width, int height, string activeText, string inactiveText, ToggleState initial)
        {

            _activeText = activeText;
            _inactiveText = inactiveText;

            WidthRequest = width;
            HeightRequest = height;

            Set(initial);
  
        }


        public void Toggle()
        {
            if (_state == ToggleState.Active)
            {
                Set(ToggleState.Inactive);
            }
            else
            {
                Set(ToggleState.Active);
            }
        }

        public void Set (ToggleState state)
        {
            switch(state)
            {
                case ToggleState.Active:
                    SetActive();
                    break;
                case ToggleState.Inactive:
                    SetInactive();
                    break;
                case ToggleState.Intermediate:
                    SetIntermediate();
                    break;
            }
            _state = state;

        }

        private void SetActive()
        {
            Label = _activeText;
            ModifyBg(StateType.Normal, _activeColor);
            ModifyBg(StateType.Prelight, _activePrelight);
        }

        private void SetInactive()
        {
            Label = _inactiveText;
            ModifyBg(StateType.Normal, _inactiveColor);
            ModifyBg(StateType.Prelight, _inactivePrelight);
        }

        private void SetIntermediate()
        {
            Label = "Intermediate";
            ModifyBg(StateType.Normal, _activeColor);
            ModifyBg(StateType.Prelight, _activePrelight);
        }

        public bool Active { get { return _state == ToggleState.Active; } }

    }
}
