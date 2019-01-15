using System;
using Gtk;
using Gdk;

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

        private Plot _plot;

        public PlotView() : base(Gtk.WindowType.Toplevel)
        {
            Title = "Data Viewer";
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);
            ModifyBg(StateType.Normal, _bgColor);

            // File and sensor view
            HBox chooseSensorBox = new HBox(false, 5);
            _fileLabel = new Label("Choose file to view sensor data");
            _fileLabel.ModifyFg(StateType.Normal, _textOnBgColor);
            _selectFileButton = new Button("Select file");
            _selectFileButton.Pressed += OnSelectFilePressed;

            _sensorStore = new ListStore(typeof(string));
            _sensorStore.AppendValues("TestName"); // Test

            CellRendererText nameCell = new CellRendererText();

            _sensorDropdown = new ComboBox();
            _sensorDropdown.PackStart(nameCell, false);
            _sensorDropdown.AddAttribute(nameCell, "text", 0);
            _sensorDropdown.Model = _sensorStore;
            _sensorDropdown.Active = 0;

            chooseSensorBox.PackStart(_fileLabel, false, false, 0);
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

            _xminEntry = new Entry { WidthChars = 6 };
            _xmaxEntry = new Entry { WidthChars = 6 };
            _yminEntry = new Entry { WidthChars = 6 };
            _ymaxEntry = new Entry { WidthChars = 6 };

            _plotButton = new Button { Label = "Plot", WidthRequest = 40 };
            _plotButton.Pressed += OnPlotButtonPressed;

            limitsBox.PackStart(xminLabel, false, false, 0);
            limitsBox.PackStart(_xminEntry, false, false, 0);

            limitsBox.PackStart(xmaxLabel, false, false, 0);
            limitsBox.PackStart(_xmaxEntry, false, false, 0);

            limitsBox.PackStart(yminLabel, false, false, 0);
            limitsBox.PackStart(_yminEntry, false, false, 0);

            limitsBox.PackStart(ymaxLabel, false, false, 0);
            limitsBox.PackStart(_ymaxEntry, false, false, 0);

            limitsBox.PackStart(_plotButton, false, false, 0);

            HBox topContainer = new HBox(false, 0);
            topContainer.PackStart(chooseSensorBox, false, false, 0);
            topContainer.PackStart(limitsBox, false, false, 0);

            // Plot view
            _plot = new Plot
            {
                Type = Plot.PlotType.PointLinePlot,
                ShowCoordinates = false,
                YLabel = "Value",
                XLabel = "Time"
            };
            _plot.Points = GetRandomData(-245, 245, -499, 600, 50);

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
                _fileLabel.Text = fileName;
            }
            else
            {

            }
        }

        private void OnPlotButtonPressed(object sender, EventArgs e)
        {

        }

        private string OpenFileDialog() {
            FileFilter filter = new FileFilter();
            filter.AddPattern(".danstar");
            FileChooserDialog fileChooser = new FileChooserDialog(
                "Choose file (format: .danstar)",
                this,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Select", ResponseType.Accept
            )
            {
                Filter = filter,
            };

            int result = fileChooser.Run();
            if (result == (int)ResponseType.Accept)
            {
                System.IO.FileStream file = System.IO.File.OpenRead(fileChooser.Filename);
                file.Close();
            }

            fileChooser.Destroy();
            return null;
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
