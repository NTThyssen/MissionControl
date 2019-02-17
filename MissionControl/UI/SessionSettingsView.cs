using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using Gtk;
using MissionControl.Data;
using MissionControl.Data.Components;
using MissionControl.UI.Widgets;

namespace MissionControl.UI
{
    public interface ISessionSettingsViewListener
    {
        void OnSave(Session session);
    }

    public partial class SessionSettingsView : Window
    {

        private ISessionSettingsViewListener _listener;
        private Session _session;

        // Choose storage for log
        Label _lblFilepath;
        Button _btnChooseFilePath;
        string _chosenFilePath;

        // Serial port
        private readonly int[] bauds = { 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000, 512000 };
        private DropdownWidget _portDropdown;
        private DropdownWidget _baudRateDropdown;
        private Button _btnPortRefresh;

        // Component limits
        List<ComponentSettingWidget> _componentWidgets = new List<ComponentSettingWidget>();

        // Fluid controls
        LabelledEntryWidget oxidValveCoefficient;
        LabelledEntryWidget oxidDensity;
        LabelledEntryWidget fuelValveCoefficient;
        LabelledEntryWidget fuelDensity;
        LabelledEntryWidget todaysPressure;
        LabelledRadioWidget showAbsolutePressure;

        // Overall actions
        Button _btnSave;

        public SessionSettingsView(ISessionSettingsViewListener handler, Session session) : base(WindowType.Toplevel)
        {
            _listener = handler;
            _session = session;
            SetSizeRequest(400, 650);
            SetPosition(WindowPosition.Center);

           //Build();
            Title = "Session Settings";

            /*
             * ------------------------------------------- Machine Page 
             */

            VBox machinePage = new VBox(false, 20);

            // Log file path
            _chosenFilePath = session.Setting.LogFilePath.Value ?? _chosenFilePath;
            

            HBox filepathContainer = new HBox(false, 0);

            _lblFilepath = new Label();
            SetFilePathLabel(_chosenFilePath);

            _btnChooseFilePath = new Button { Label = "Choose" };
            _btnChooseFilePath.Pressed += ChooseFilePathPressed;

            filepathContainer.PackStart(_btnChooseFilePath, false, false, 10);
            filepathContainer.PackStart(_lblFilepath, false, false, 0);

            // Select serial port
            HBox portBox = new HBox(false, 10);
            _portDropdown = new DropdownWidget(null);
            PortRefreshPressed(null, null);
            if (session.Setting.PortName.Value != null)
            {
                _portDropdown.Set(session.Setting.PortName.Value);
            }

            _btnPortRefresh = new Button(Stock.Refresh);
            _btnPortRefresh.Pressed += PortRefreshPressed;
            portBox.PackStart(_portDropdown, false, false, 10);
            portBox.PackStart(_btnPortRefresh, false, false, 0);

            _baudRateDropdown = new DropdownWidget("Baud rate", bauds.Select(x => x.ToString()).ToArray());
            _baudRateDropdown.Set(session.Setting.BaudRate.Value.ToString());

            machinePage.PackStart(new Label("Serial port:"), false, false, 10);
            machinePage.PackStart(portBox, false, false, 0);
            machinePage.PackStart(_baudRateDropdown, false, false, 10);
            machinePage.PackStart(new Label("Save path:"), false, false, 0);
            machinePage.PackStart(filepathContainer, false, false, 0);

            /*
             * ------------------------------------------- Visual Page 
             */

            VBox visualPage = new VBox(false, 10);

            // Component limits

            _session.Mapping.Components().ForEach((Component obj) =>
            {
                if (obj is IWarningLimits sc)
                {
                    _componentWidgets.Add(new ComponentSettingWidget(obj));
                }
            });

            VBox componentSections = new VBox(false, 20);
            componentSections.PackStart(LayoutLimitsSection(_componentWidgets.FindAll((ComponentSettingWidget obj) => obj.Component is PressureComponent)), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(_componentWidgets.FindAll((ComponentSettingWidget obj) => obj.Component is TemperatureComponent)), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(_componentWidgets.FindAll((ComponentSettingWidget obj) => obj.Component is LoadComponent)), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(_componentWidgets.FindAll((ComponentSettingWidget obj) => obj.Component is VoltageComponent)), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(_componentWidgets.FindAll((ComponentSettingWidget obj) => obj.Component is FlowComponent)), false, false, 0);

            ScrolledWindow scrolledWindow = new ScrolledWindow
            {
                HeightRequest = 550
            };
            //scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            scrolledWindow.AddWithViewport(componentSections);

            visualPage.PackStart(scrolledWindow, false, false, 0);

            /*
             * -------------------------------------------  Fluid Page
             */

            VBox fluidPage = new VBox(false, 20);

            oxidValveCoefficient = new LabelledEntryWidget()
            {
                LabelText = "Oxidizer valve flow coefficient (CV)",
                EntryText = session.Setting.OxidCV.ToString() ?? ""
            };
                       
            oxidDensity = new LabelledEntryWidget()
            {
                LabelText = "Oxidizer density [g/L]",
                EntryText = session.Setting.OxidDensity.ToString() ?? ""
            };

            fuelValveCoefficient = new LabelledEntryWidget()
            {
                LabelText = "Fuel valve flow coefficient (CV)",
                EntryText = session.Setting.FuelCV.ToString() ?? ""
            };

            fuelDensity = new LabelledEntryWidget()
            {
                LabelText = "Fuel density [g/L]",
                EntryText = session.Setting.FuelDensity.ToString() ?? ""
            };

            todaysPressure = new LabelledEntryWidget()
            {
                LabelText = "Today's pressure [bar]",
                EntryText = session.Setting.TodayPressure.ToString() ?? ""
            };

            showAbsolutePressure = new LabelledRadioWidget
            {
                LabelText = "Pressure measure: ",
                ShowAbsolutePressure = session.Setting.ShowAbsolutePressure.Value
            };

            fluidPage.PackStart(new Label("Fluid system values"), false, false, 0);

            fluidPage.PackStart(oxidValveCoefficient, false, false, 0);
            fluidPage.PackStart(oxidDensity, false, false, 0);

            fluidPage.PackStart(fuelValveCoefficient, false, false, 0);
            fluidPage.PackStart(fuelDensity, false, false, 0);

            fluidPage.PackStart(new Label("Pressure properties"), false, false, 0);

            fluidPage.PackStart(todaysPressure, false, false, 0);
            fluidPage.PackStart(showAbsolutePressure, false, false, 0);

            /*
             * ------------------------------------------- Overall
             */

            VBox container = new VBox(false, 10);

            Notebook notebook = new Notebook
            {
                machinePage,
                visualPage,
                fluidPage
            };
            notebook.SetTabLabelText(machinePage, "Machine");
            notebook.SetTabLabelText(visualPage, "Visuals");
            notebook.SetTabLabelText(fluidPage, "Fluid system");

            // Bottom save container
            HBox cancelSaveContainer = new HBox(false, 0);
            _btnSave = new Button {
                Label = "Save",
                WidthRequest = 60,
                HeightRequest = 40
            };
            _btnSave.Pressed += SavePressed;
            cancelSaveContainer.PackEnd(_btnSave, false, false, 10);

            container.PackStart(notebook, false, false, 0);
            container.PackStart(cancelSaveContainer, false, false, 0);

            Add(container);
            Resizable = false;

            ShowAll();
        }

        private VBox LayoutLimitsSection (List<ComponentSettingWidget> components)
        {
            if (components.Count == 0) return new VBox(false, 0);

            VBox section = new VBox(false, 10);
            Label typename = new Label { 
                Text = components[0].Component.TypeName + " sensors:"
            };
            section.PackStart(typename, false, false, 10);

            HBox[] boxes = new HBox[components.Count / 2 + 1];
            for (int i = 0; i < components.Count; i++)
            {
                if (i % 2 == 0)
                {
                    boxes[i/2] = new HBox(false, 30);
                    boxes[i / 2].PackStart(components[i], false, false, 0);
                }
                else
                {
                    boxes[i / 2].PackStart(components[i]);
                    section.PackStart(boxes[i / 2], false, false, 0);
                }
            }
            if (components.Count == 1)
            {   
                section.PackStart(boxes[0], false, false, 0);
            }

            return section;
        }

        void PortRefreshPressed(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            _portDropdown.AddOnlyAll(ports);
        }

        void ChooseFilePathPressed(object sender, EventArgs e)
        {
            _chosenFilePath = OpenFileDialog();
            SetFilePathLabel(_chosenFilePath);
        }

        void SetFilePathLabel(string path)
        {
            if (path != null)
            {
                _lblFilepath.Text = "...";

                DirectoryInfo info = new DirectoryInfo(path);
                DirectoryInfo parent = info.Parent;

                if (parent != null)
                {
                    DirectoryInfo parentsParent = parent.Parent;
                    if (parentsParent != null)
                    {
                        _lblFilepath.Text += (parentsParent != null) ? "/" + parentsParent.Name : string.Empty;
                    }
                    _lblFilepath.Text += (parent != null) ? "/" + parent.Name : string.Empty;
                }
                _lblFilepath.Text += "/" + info.Name;


            }
            else
            {
                _lblFilepath.Text = "Press choose to select where to save log";
            }
        }


        void SavePressed(object sender, EventArgs e)
        {
            Session newSession = new Session(_session.Mapping);

            bool hasErrors = false;
            string errorMessage = "";

            // Verify user has selected save path
            if (_chosenFilePath == null)
            {
                hasErrors = true;
                errorMessage += "- File path to save log not chosen\n";
            }
            else
            {
                newSession.Setting.LogFilePath.Value = _chosenFilePath;

            }

            // Verify user has selected serial port
            if (_portDropdown.Active == -1)
            {
                hasErrors = true;
                errorMessage += "- No serial port selected\n";
            }
            else
            {
                newSession.Setting.PortName.Value = _portDropdown.ActiveText;
            }

            // Save chosen baud rate
            if (int.TryParse(_baudRateDropdown.ActiveText, out int baud))
            {
                newSession.Setting.BaudRate.Value = baud;
            }
            else
            {
                hasErrors = true;
                errorMessage += "- Conversion error in baud rate\n";
            }


            // Update sensor component settings
            foreach (ComponentSettingWidget widget in _componentWidgets)
            {
                if (ValidateSensorLimits(widget, out float min, out float max, out string err))
                {
                    hasErrors = true;
                    errorMessage += err;
                }
                else
                {
                    if (newSession.Mapping.ComponentsByID()[widget.Component.BoardID] is IWarningLimits sc)
                    {
                        sc.MinLimit = min;
                        sc.MaxLimit = max;
                    }
                }

                newSession.Mapping.ComponentsByID()[widget.Component.BoardID].Enabled = widget.Enabled;
            }

            // Fluid page's field validation
            newSession.Setting.OxidCV.Value = ParseLabellelEntryAsFloat(oxidValveCoefficient, ref hasErrors, ref errorMessage, out float oxcv) ? oxcv : newSession.Setting.OxidCV.Value;
            newSession.Setting.OxidDensity.Value = ParseLabellelEntryAsFloat(oxidDensity, ref hasErrors, ref errorMessage, out float oxd) ? oxd : newSession.Setting.OxidDensity.Value;

            newSession.Setting.FuelCV.Value = ParseLabellelEntryAsFloat(fuelValveCoefficient, ref hasErrors, ref errorMessage, out float flcv) ? flcv : newSession.Setting.OxidCV.Value;
            newSession.Setting.FuelDensity.Value = ParseLabellelEntryAsFloat(fuelDensity, ref hasErrors, ref errorMessage, out float fld) ? fld : newSession.Setting.FuelDensity.Value;

            newSession.Setting.TodayPressure.Value = ParseLabellelEntryAsFloat(todaysPressure, ref hasErrors, ref errorMessage, out float tp) ? tp : newSession.Setting.OxidCV.Value;
            newSession.Setting.ShowAbsolutePressure.Value = showAbsolutePressure.ShowAbsolutePressure;

            // Save if no errors
            if (!hasErrors)
            {
                _listener.OnSave(newSession);
            }
            else
            {
                MessageDialog errorDialog = new MessageDialog(this, 
                    DialogFlags.DestroyWithParent, 
                    MessageType.Error, 
                    ButtonsType.Close, 
                    errorMessage
                );
                errorDialog.Run();
                errorDialog.Destroy();
            }
        }

        bool ValidateSensorLimits(ComponentSettingWidget widget, out float min, out float max, out string errorMessage)
        {
            bool error = false;
            errorMessage = "";
            min = float.NaN;
            max = float.NaN;

            if (widget.Min.Length > 0)
            {
                try
                {
                    min = Convert.ToSingle(widget.Min);
                } catch (Exception)
                {
                    error = true;
                    errorMessage += string.Format("Minimum is not a correct value for {0}\n", widget.Component.Name);
                }
            }
         

            if (widget.Max.Length > 0)
            {
                try
                {
                    max = Convert.ToSingle(widget.Max);
                }
                catch (Exception)
                {
                    error = true;
                    errorMessage += string.Format("Maximum is not a correct value for {0}\n", widget.Component.Name);
                }
            }


            return error;
        }

        public bool ParseLabellelEntryAsFloat(LabelledEntryWidget entry, ref bool errors, ref string errmsg, out float val)
        {
            if (float.TryParse(entry.EntryText, NumberStyles.Any, CultureInfo.InvariantCulture,  out float value))
            {
                val = value;
                return true;
            }
            else
            {
                val = float.NaN;
                errors = true;
                errmsg += "- Could not parse \"" + entry.LabelText + "\" field\n";
                return false;
            }
        }

        private string OpenFileDialog()
        {
            FileChooserDialog fileChooser = new FileChooserDialog(
                "Choose where to save",
                this,
                FileChooserAction.SelectFolder,
                "Cancel", ResponseType.Cancel,
                "Select", ResponseType.Accept
            );
                
            int result = fileChooser.Run();
            string filepath = null;

            if (result == (int)ResponseType.Accept)
            {
                filepath = fileChooser.Filename;
            }

            fileChooser.Destroy();

            return filepath;
        }
    }
}