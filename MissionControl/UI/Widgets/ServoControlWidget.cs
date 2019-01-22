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

        private Entry _setInput;

        private ServoCallback _callback;
        private ServoComponent _component;

        public ServoControlWidget(ServoComponent component, ServoCallback callback)
        {
            Build();

            _component = component;
            _callback = callback;

            VisibleWindow = false;
            AboveChild = false;

            Button openButton = new Button
            {
                Label = "Open",
                HeightRequest = 40
            };

            Button closeButton = new Button
            {
                Label = "Close",
                HeightRequest = 40
            };

            Button setButton = new Button
            {
                Label = "Set",
                HeightRequest = 15
            };

            _setInput = new Entry {
                WidthChars = 5,
                HeightRequest = 40,
            };

            openButton.Pressed += OpenButton_Pressed;
            closeButton.Pressed += CloseButton_Pressed;
            setButton.Pressed += SetButton_Pressed;

            HBox controls = new HBox(false, 5);
            controls.PackStart(openButton, false, false, 0);
            controls.PackStart(closeButton, false, false, 0);
            controls.PackStart(_setInput, false, false, 0);
            controls.PackStart(setButton, false, false, 0);

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

            Add(container);

        }

        void SetButton_Pressed(object sender, EventArgs e)
        {
            string input = _setInput.Text;
            input = input.Replace(',', '.');

            float value;

            try
            {
                value = float.Parse(input, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (Exception err)
            {
                if (Toplevel.IsTopLevel)
                {
                    Window top = (Window) Toplevel;
                    MessageDialog errorDialog = new MessageDialog(top,
                    DialogFlags.DestroyWithParent,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Value for Valve is not a number value"
                    );
                    errorDialog.Run();
                    errorDialog.Destroy();
                    return;
                }

                Console.WriteLine("Value for Valve is not a number value");
                return;
            }

            if (value <= 100.0f && value >= 0.0f)
            {
                _callback(_component, value);
            }
            else
            {
                if (Toplevel.IsTopLevel)
                {
                    Window top = (Window)Toplevel;
                    MessageDialog errorDialog = new MessageDialog(top,
                    DialogFlags.DestroyWithParent,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Value not between 0.0 and 100.0"
                    );
                    errorDialog.Run();
                    errorDialog.Destroy();
                    return;
                }
                Console.WriteLine("Value not between 0.0 and 100.0");
                return;
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

    }
}
