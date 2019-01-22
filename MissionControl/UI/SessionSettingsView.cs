using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
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

        // Choose serial port
        private ComboBox _portDropdown;
        private ListStore _portStore;
        private List<string> _portList;
        private Button _btnPortRefresh;

        // Overall actions
        Button _btnSave;

        public SessionSettingsView(ISessionSettingsViewListener handler, Session session) : base(WindowType.Toplevel)
        {
            _listener = handler;
            _session = session;
            Build();
            Title = "Session Settings";


            VBox container = new VBox(false, 10);

            // Log file path
            if (session.LogFilePath != null)
            {
                _chosenFilePath = session.LogFilePath;
            }
            else if (PreferenceManager.Preferences[PreferenceManager.STD_LOGFOLDER] != null)
            {
                _chosenFilePath = PreferenceManager.Preferences[PreferenceManager.STD_LOGFOLDER];
            }

            HBox filepathContainer = new HBox(false, 0);

            _lblFilepath = new Label();
            SetFilePathLabel(_chosenFilePath);

            _btnChooseFilePath = new Button { Label = "Choose" };
            _btnChooseFilePath.Pressed += ChooseFilePathPressed;

            filepathContainer.PackStart(_btnChooseFilePath, false, false, 10);
            filepathContainer.PackStart(_lblFilepath, false, false, 0);

            // Select serial port
            HBox portBox = new HBox(false, 10);
            _portStore = new ListStore(typeof(string));
            _portList = new List<string>(); 
            PortRefreshPressed(null, null);

            CellRendererText nameCell = new CellRendererText();

            _portDropdown = new ComboBox();
            _portDropdown.PackStart(nameCell, false);
            _portDropdown.AddAttribute(nameCell, "text", 0);
            _portDropdown.Model = _portStore;

            if (session.PortName != null && _portList.Contains(session.PortName))
            {
                _portDropdown.Active = _portList.IndexOf(session.PortName);
            }
            else
            {
                string savedPort = PreferenceManager.Preferences[PreferenceManager.STD_PORTNAME];
                if (savedPort != null && _portList.Contains(savedPort))
                {
                    _portDropdown.Active = _portList.IndexOf(savedPort);
                }
            }

            _btnPortRefresh = new Button(Stock.Refresh);
            _btnPortRefresh.Pressed += PortRefreshPressed;
            portBox.PackStart(_portDropdown, false, false, 10);
            portBox.PackStart(_btnPortRefresh, false, false, 0);

            // Component limits
            List<ComponentSettingWidget> pressures = new List<ComponentSettingWidget>();
            List<ComponentSettingWidget> temperatures = new List<ComponentSettingWidget>();
            List<ComponentSettingWidget> loads = new List<ComponentSettingWidget>();
            List<ComponentSettingWidget> voltages = new List<ComponentSettingWidget>();

            foreach (Component component in _session.Mapping.Components())
            {
                switch (component)
                {
                    case PressureComponent p: pressures.Add(new ComponentSettingWidget(p)); break;
                    case TemperatureComponent t: temperatures.Add(new ComponentSettingWidget(t)); break;
                    case LoadComponent l: loads.Add(new ComponentSettingWidget(l)); break;
                    case VoltageComponent v: voltages.Add(new ComponentSettingWidget(v)); break;
                }
            }

            VBox componentSections = new VBox(false, 20);
            componentSections.PackStart(LayoutLimitsSection(pressures), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(temperatures), false, false,0);
            componentSections.PackStart(LayoutLimitsSection(loads), false, false, 0);
            componentSections.PackStart(LayoutLimitsSection(voltages), false, false, 0);

            // Bottom save container
            HBox cancelSaveContainer = new HBox(false, 0);
            _btnSave = new Button { Label = "Save" };
            _btnSave.Pressed += SavePressed;
            cancelSaveContainer.PackEnd(_btnSave, false, false, 10);

            container.PackStart(new Label("Serial port:"), false, false, 10);
            container.PackStart(portBox, false, false, 0);
            container.PackStart(new Label("Save path:"), false, false, 0);
            container.PackStart(filepathContainer, false, false, 0);
            container.PackStart(componentSections, false, false, 0);
            container.PackStart(cancelSaveContainer, false, false, 0);


            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            scrolledWindow.AddWithViewport(container);

            Add(scrolledWindow);

            SetSizeRequest(400, 650);
            SetPosition(WindowPosition.Center);
            ShowAll();
        }

        private VBox LayoutLimitsSection (List<ComponentSettingWidget> components)
        {
            if (components.Count == 0) return new VBox(false, 0);

            VBox section = new VBox(false, 0);
            Label typename = new Label { 
                Text = components[0].Component.TypeName + " sensor limit warnings:"
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
            _portStore.Clear();
            _portList.Clear();

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                _portList.Add(port);
                _portStore.AppendValues(port);
            }
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
                newSession.LogFilePath = _chosenFilePath;

            }

            // Verify user has selected serial port
            if (_portDropdown.Active == -1)
            {
                hasErrors = true;
                errorMessage += "- No serial port selected\n";
            }
            else
            {
                newSession.PortName = _portDropdown.ActiveText;
            }

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