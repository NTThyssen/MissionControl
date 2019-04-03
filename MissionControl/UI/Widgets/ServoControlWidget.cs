using System;
using System.Globalization;
using Gtk;
using MissionControl.Data.Components;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ServoControlWidget : EventBox
    {
        public delegate void ServoCallback(ServoComponent component, float value); 
        public delegate void ServoCallbackTimed(ServoComponent component, float startValue, float endValue, int delayMillis); 

        private Entry _setInput;

        private ServoCallback _callback;
        private ServoCallbackTimed _timedCallback;
        private ServoComponent _component;

        private Button _openButton, _closeButton, _setButton;
        private Button _setTimeButton;
        private Entry _timeEntry;

        public ServoControlWidget(ServoComponent component, ServoCallback callback, ServoCallbackTimed timedCallback)
        {
            Build();

            _component = component;
            _callback = callback;
            _timedCallback = timedCallback;

            VisibleWindow = false;
            AboveChild = false;

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
                HeightRequest = 40
            };

            _setInput = new Entry 
            {
                WidthChars = 5,
                HeightRequest = 40,
            };

            _setTimeButton = new Button
            {
                Label = "Set with time",
                HeightRequest = 40
            };

            _timeEntry = new Entry
            {
                WidthChars = 6,
                HeightRequest = 40
            };
            
            _openButton.Pressed += OpenButton_Pressed;
            _closeButton.Pressed += CloseButton_Pressed;
            _setButton.Pressed += SetButton_Pressed;
            _setTimeButton.Pressed += SetTimeButtonOnPressed;

            Gdk.Color insensitiveColor = new Gdk.Color(140, 140, 140);
            _openButton.ModifyBg(StateType.Insensitive, insensitiveColor);
            _closeButton.ModifyBg(StateType.Insensitive, insensitiveColor);
            _setButton.ModifyBg(StateType.Insensitive, insensitiveColor);
            _setInput.ModifyBase(StateType.Insensitive, insensitiveColor);
            _setTimeButton.ModifyBg(StateType.Insensitive, insensitiveColor);
            _timeEntry.ModifyBase(StateType.Insensitive, insensitiveColor);

            HBox controls = new HBox(false, 5);
            controls.PackStart(_openButton, false, false, 0);
            controls.PackStart(_closeButton, false, false, 0);
            controls.PackStart(_setInput, false, false, 0);
            controls.PackStart(_setButton, false, false, 0);
            
            HBox timedControls = new HBox(false, 5);
            timedControls.PackStart(_timeEntry, false, false, 0);
            timedControls.PackStart(_setTimeButton, true, true, 0);

            VBox container = new VBox(false, 5);

            Label name = new Label()
            {
                Text = _component.Name,
                Xalign = 0.0f,
                Yalign = 0.0f
            };
            name.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

            container.PackStart(name, false, false, 0);
            container.PackStart(controls, false, false, 0);
            container.PackStart(timedControls, false, false, 0);

            Add(container);

        }

        private void SetTimeButtonOnPressed(object sender, EventArgs e)
        {   
            if (float.TryParse(_setInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float startValue))
            {
                if (int.TryParse(_timeEntry.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out int timeValue))
                {
                    if (startValue <= 100.0f && startValue >= 0.0f)
                    {
                        if (timeValue > 0)
                        {
                            _timedCallback(_component, startValue, _component.ClosePosition, timeValue);
                        }
                        else
                        {
                            ShowErrorMessage("Time value is not over 0");
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Value for Valve is not between 0.0 and 100.0");
                    }
                }
                else
                {
                    ShowErrorMessage("Value for time is not an integer");
                }
            }
            else
            {
                ShowErrorMessage("Value for Valve is not a float value");
            }
        }

        void SetButton_Pressed(object sender, EventArgs e)
        {
            string input = _setInput.Text;
            input = input.Replace(',', '.');

            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                if (value <= 100.0f && value >= 0.0f)
                {
                    _callback(_component, value);
                }
                else
                {
                    ShowErrorMessage("Value not between 0.0 and 100.0");
                }
            }
            else
            {
                ShowErrorMessage("Value for Valve is not a number value");
            }
        }

        void OpenButton_Pressed(object sender, EventArgs e)
        {
            _callback(_component, _component.OpenPosition);
        }

        void CloseButton_Pressed(object sender, EventArgs e)
        {
            _callback(_component, _component.ClosePosition);
        }

        private void ShowErrorMessage(string msg)
        {
            if (Toplevel.IsTopLevel)
            {
                Window top = (Window)Toplevel;
                MessageDialog errorDialog = new MessageDialog(top,
                    DialogFlags.DestroyWithParent,
                    MessageType.Error,
                    ButtonsType.Close,
                    msg
                );
                errorDialog.Run();
                errorDialog.Destroy();
                return;
            }
            Console.WriteLine(msg);
        }

    }
}
