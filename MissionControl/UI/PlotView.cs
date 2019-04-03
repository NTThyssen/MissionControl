using System;
using Gtk;
using Gdk;
using System.Collections.Generic;
using MissionControl.Data;
using System.IO;

namespace MissionControl.UI
{
    public partial class PlotView : Gtk.Window
    {

        private Color _bgColor = new Color(0, 0, 0);
        private Color _textOnBgColor = new Color(255, 255, 255);

        private Label _fileLabel;
        private Button _selectFileButton;
        private ComboBox _sensorDropdown;
        private ListStore _sensorStore;

        private Entry _xminEntry, _xmaxEntry, _yminEntry, _ymaxEntry;
        private Button _plotButton;

        private PlotData _data;
        private Plot _plot;

        public PlotView() : base(Gtk.WindowType.Toplevel)
        {
            Title = "Data Viewer";
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);
            ModifyBg(StateType.Normal, _bgColor);

            // File and sensor view
            HBox chooseSensorBox = new HBox(false, 5);
            _fileLabel = new Label
            {
                Text = "Choose file to view sensor data",
                MaxWidthChars = 30
            };
            _fileLabel.ModifyFg(StateType.Normal, _textOnBgColor);
            _selectFileButton = new Button("Select file");
            _selectFileButton.Pressed += OnSelectFilePressed;

            _sensorStore = new ListStore(typeof(string));

            CellRendererText nameCell = new CellRendererText();

            _sensorDropdown = new ComboBox();
            _sensorDropdown.PackStart(nameCell, false);
            _sensorDropdown.AddAttribute(nameCell, "text", 0);
            _sensorDropdown.Model = _sensorStore;
            _sensorDropdown.Sensitive = false;
            _sensorDropdown.WidthRequest = 80;
            _sensorDropdown.Changed += SensorDropdownChanged;

            chooseSensorBox.PackStart(_fileLabel, false, false, 10);
            chooseSensorBox.PackStart(_selectFileButton, false, false, 0);
            chooseSensorBox.PackStart(_sensorDropdown, false, false, 20);

            // Plot limits view
            HBox limitsBox = new HBox(false, 5);
            Label xminLabel = new Label("X Min: ");
            Label xmaxLabel = new Label("X Max: ");
            Label yminLabel = new Label("Y Min: ");
            Label ymaxLabel = new Label("Y Max: ");

            xminLabel.ModifyFg(StateType.Normal, _textOnBgColor);
            xmaxLabel.ModifyFg(StateType.Normal, _textOnBgColor);
            yminLabel.ModifyFg(StateType.Normal, _textOnBgColor);
            ymaxLabel.ModifyFg(StateType.Normal, _textOnBgColor);

            _xminEntry = new Entry { WidthChars = 6, Sensitive = false };
            _xmaxEntry = new Entry { WidthChars = 6, Sensitive = false };
            _yminEntry = new Entry { WidthChars = 6, Sensitive = false };
            _ymaxEntry = new Entry { WidthChars = 6, Sensitive = false };

            _plotButton = new Button { Label = "Plot", WidthRequest = 40 };
            _plotButton.Sensitive = false;
            _plotButton.Pressed += OnPlotButtonPressed;

            limitsBox.PackStart(xminLabel, false, false, 0);
            limitsBox.PackStart(_xminEntry, false, false, 0);

            limitsBox.PackStart(xmaxLabel, false, false, 0);
            limitsBox.PackStart(_xmaxEntry, false, false, 0);

            limitsBox.PackStart(yminLabel, false, false, 0);
            limitsBox.PackStart(_yminEntry, false, false, 0);

            limitsBox.PackStart(ymaxLabel, false, false, 0);
            limitsBox.PackStart(_ymaxEntry, false, false, 0);

            limitsBox.PackStart(_plotButton, false, false, 10);

            HBox topContainer = new HBox(false, 0);
            topContainer.PackStart(chooseSensorBox, false, false, 0);
            topContainer.PackStart(limitsBox, false, false, 0);

            // Plot view
            _plot = new Plot
            {
                Type = Plot.PlotType.PointLinePlot,
                ShowCoordinates = false,
                XLabel = "Time"
            };
            //_plot.Points = GetRandomData(-245, 245, -499, 600, 50);
            _plot.Paint();

            VBox container = new VBox(false, 10);
            container.PackStart(topContainer, false, false, 10);
            container.PackStart(_plot, true, true, 0);

            Add(container);

            ShowAll();
        }

        private void OnSelectFilePressed(object sender, EventArgs e)
        {
            string fileName = OpenFileDialog();
            if (fileName != null)
            {

                _fileLabel.Text = (fileName.Length > 30) ? "..." + fileName.Substring(fileName.Length - 30 -1 ) : fileName;

                StreamReader file = new StreamReader(fileName);
                _data = FormatPretty.PrettyToData(file);
                _sensorStore.Clear();
                foreach (KeyValuePair<string, List<float>> sensor in _data.Values)
                {
                    _sensorStore.AppendValues(sensor.Key);
                }

                _sensorDropdown.Sensitive = true;
            }
            else
            {

            }
        }

        private void OnPlotButtonPressed(object sender, EventArgs e)
        {
            if (_sensorDropdown.Active != -1)
            {
                string selected = _sensorDropdown.ActiveText;
                if (_data.Values.ContainsKey(selected))
                {

                    _plot.Points = _data.ToPoints(selected);
                    _plot.YLabel = selected;

                    float xmin, xmax, ymin, ymax;
                    try
                    {
                        xmin = Convert.ToSingle(_xminEntry.Text);
                      } catch (Exception)
                    {
                        ErrorDialog(string.Format("The input {0} in X Min could not be converted to float", _xminEntry.Text));
                        return;
                    }

                    try
                    {
                        xmax = Convert.ToSingle(_xmaxEntry.Text);
                    }
                    catch (Exception)
                    {
                        ErrorDialog(string.Format("The input {0} in X Max could not be converted to float", _xmaxEntry.Text));
                        return;
                    }

                    try
                    {
                        ymin = Convert.ToSingle(_yminEntry.Text);
                    }
                    catch (Exception)
                    {
                        ErrorDialog(string.Format("The input {0} in Y Min could not be converted to float", _yminEntry.Text));
                        return;
                    }

                    try
                    {
                        ymax = Convert.ToSingle(_ymaxEntry.Text);
                    }
                    catch (Exception)
                    {
                        ErrorDialog(string.Format("The input {0} in Y Max could not be converted to float", _ymaxEntry.Text));
                        return;
                    }

                    _plot.Xmin = xmin;
                    _plot.Xmax = xmax;
                    _plot.Ymin = ymin;
                    _plot.Ymax = ymax;
                    _plot.Paint();
                }

            }
        }

        void SensorDropdownChanged(object sender, EventArgs e)
        {
            if (_sensorDropdown.Active != -1)
            {
                string selected = _sensorDropdown.ActiveText;
                if (_data.Values.ContainsKey(selected))
                {

                    _plot.Boundaries(_data.ToPoints(selected), out float xmin, out float xmax, out float ymin, out float ymax);

                    _xminEntry.Sensitive = true;
                    _xmaxEntry.Sensitive = true;
                    _yminEntry.Sensitive = true;
                    _ymaxEntry.Sensitive = true;

                    _xminEntry.Text = "" + Math.Round(xmin, 2);
                    _xmaxEntry.Text = "" + Math.Round(xmax, 2);
                    _yminEntry.Text = "" + Math.Round(ymin, 2);
                    _ymaxEntry.Text = "" + Math.Round(ymax, 2);

                    _plotButton.Sensitive = true;
                    return;
                }
              
            }
            _plotButton.Sensitive = false;
        }


        private string OpenFileDialog() {
            FileFilter filter = new FileFilter();
            filter.AddPattern("*.csv");
            filter.AddPattern("*.danstar");
            FileChooserDialog fileChooser = new FileChooserDialog(
                "Choose file (format: .csv or .danstar)",
                this,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Select", ResponseType.Accept
            )
            {
                Filter = filter
            };

            int result = fileChooser.Run();
            if (result == (int)ResponseType.Accept)
            {
                string filename = fileChooser.Filename;
                fileChooser.Destroy();
                return filename;
            }

            fileChooser.Destroy();
            return null;
        }

        void ErrorDialog(string errorMessage)
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

        private Plot.Point[] GetFakeLinData(int min, int max, int volatility, int n)
        {
            Plot.Point[] points = new Plot.Point[n];
            Random random = new Random();

            double last = random.NextDouble() * (Math.Abs(min) + Math.Abs(max)) - Math.Abs(min);
            for (int i = 0; i < n; i++)
            {
                last += random.NextDouble() * volatility;
                points[i] = new Plot.Point(i, (float) (last + last));
               //points[i] = new Plot.Point(i, random.Next(min, max));
            }
            return points;
        }

        private Plot.Point[] GetRandomData(int xmin, int xmax, int ymin, int ymax, int n)
        {
            Plot.Point[] points = new Plot.Point[n];
            Random random = new Random();
            for (int i = n - 1; i >= 0; i--)
            {
                points[i] = new Plot.Point(random.Next(xmin, xmax), random.Next(ymin, ymax));
                //points[i] = new Plot.Point(i, random.Next(min, max));
            }
            return points;
        }

        /*
            plot.Points = GetFakeLinData(0, 1000, 4, 15);
            plot.Points = GetRandomData(-245, 245, -499, 600, 50);
            plot.Points = new Plot.Point[] { new Plot.Point(-30,-30), new Plot.Point(30, -30), new Plot.Point(-30, 30), new Plot.Point(30, 30) };
            plot.Points = new Plot.Point[] { new Plot.Point(-30, -30), new Plot.Point(-30, 30) };
            plot.Points = new Plot.Point[] { new Plot.Point(30, -30), new Plot.Point(30, 30) };
        */
    }

}
