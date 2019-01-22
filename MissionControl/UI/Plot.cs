using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace MissionControl.UI
{
    public class Plot : DrawingArea
    {

        public enum PlotType
        {
            PointPlot,
            LinePlot,
            PointLinePlot
        }

        Point[] _points;

        public Point[] Points
        {
            get { return _points; }
            set {
                Xmin = float.NaN;
                Xmax = float.NaN;
                Ymin = float.NaN;
                Ymax = float.NaN;
                _points = value;
            } 
        }

        public float Xmin { get; set; } = float.NaN;
        public float Xmax { get; set; } = float.NaN;
        public float Ymin { get; set; } = float.NaN;
        public float Ymax { get; set; } = float.NaN;
        public PlotType Type { get; set; } = PlotType.PointPlot;
        public int PixelsPerTick { get; set; } = 60;
        public bool ShowCoordinates { get; set; } = false;
        public Color BckColor { get; set; } = new Color(0, 0, 0);
        public Color FrtColor { get; set; } = new Color(1, 1, 1);
        public Color AxisColor { get; set; } = new Color(0.5, 0.5, 0.5);
        public bool YPad { get; set; } = true;

        public String XLabel { get; set; } = null;
        public String YLabel { get; set; } = null;

        public Plot()
        {
            ExposeEvent += OnExpose;
        }

        void OnExpose(object sender, ExposeEventArgs args)
        {
            //Paint();
            Console.WriteLine("Plot On Expose!");
        }

        public void Paint()
        {
            if (Points == null)
            {
                ModifyBg(StateType.Normal, new Gdk.Color(0,0,0));
                return;
            }

            Boundaries(Points, out float cXmin, out float cXmax, out float cYmin, out float cYmax);
            float ypad = Math.Abs((float.IsNaN(Ymax) ? cYmax : Ymax) - (float.IsNaN(Ymin) ? cYmin : Ymin)) * 0.15f;
            Xmin = (float.IsNaN(Xmin)) ? cXmin : Xmin;
            Xmax = (float.IsNaN(Xmax)) ? cXmax : Xmax;
            Ymin = (float.IsNaN(Ymin)) ? cYmin - ypad : Ymin;
            Ymax = (float.IsNaN(Ymax)) ? cYmax + ypad : Ymax;
            
            if (Xmin == Xmax)
            {
                Xmin -= 10;
                Xmax += 10;
            }

            if (Ymin == Ymax)
            {
                Ymin -= 10;
                Ymax += 10;
            }

            // Limit by x and y
            Point[] points = Array.FindAll(Points, p => p.X >= Xmin && p.X <= Xmax && p.Y >= Ymin && p.Y <= Ymax);
            Array.Sort(points);

            int paddingTop = 20;
            int paddingRight = 20;
            int paddingBottom = (XLabel != null) ? 60 : 20;
            int paddingLeft = (YLabel != null) ? 80 : 50;

            int width = Allocation.Width;
            int height = Allocation.Height;

            int xAxisWidth = width - paddingLeft - paddingRight;
            int yAxisHeight = height - paddingTop - paddingBottom;

            int xSpan = (int) Math.Abs(Xmax - Xmin);
            int ySpan = (int ) Math.Abs(Ymax - Ymin);

            float xRatio = (float)xAxisWidth / xSpan;
            float yRatio = (float)yAxisHeight / ySpan;

            float xOrigo = paddingLeft - Xmin * xRatio;
            float yOrigo = paddingTop + Ymax * yRatio;

            int xTicks = xAxisWidth / PixelsPerTick;
            int yTicks = yAxisHeight / PixelsPerTick;



            Context cr = Gdk.CairoHelper.Create(this.GdkWindow);

            // Draw background
            cr.SetSourceColor(BckColor);
            cr.Rectangle(0, 0, width, height);
            cr.Fill();

            // Draw bounding box
            cr.LineWidth = 2;
            cr.SetSourceColor(FrtColor);
            cr.Rectangle(paddingLeft, paddingTop, xAxisWidth, yAxisHeight);
            cr.Stroke();

            // Draw x axis
            if (Ymin < 0 && Ymax > 0)
            {
                cr.LineWidth = 1;
                cr.SetSourceColor(AxisColor);
                cr.MoveTo(paddingLeft, yOrigo);
                cr.LineTo(paddingLeft + xAxisWidth, yOrigo);
                cr.Stroke();
            }

            // Draw x ticks and values
            cr.SetSourceColor(FrtColor);
            cr.LineWidth = 2;
            List<Tick> xTicksList = CalculateTicks(Xmin, Xmax, xTicks);
            foreach (Tick tick in xTicksList)
            {
                float x = tick.Position * xRatio + xOrigo;
                cr.MoveTo(x, paddingTop + yAxisHeight - 5);
                cr.RelLineTo(0, 10);

                TextExtents te = cr.TextExtents(tick.Value);

                cr.RelMoveTo(-0.5 * te.Width, 10);
                cr.ShowText(tick.Value);
            }
            cr.Stroke();

            // Draw x label
            if (XLabel != null)
            {
                cr.Save();
                cr.SetFontSize(18);
                TextExtents te = cr.TextExtents(XLabel);
                cr.MoveTo(paddingLeft + xAxisWidth / 2 - te.Width / 2, height - te.Height);
                cr.ShowText(XLabel);
                cr.Restore();
            }

            // Draw y axis
            if (Xmin < 0 && Xmax > 0)
            {
                cr.SetSourceColor(AxisColor);
                cr.MoveTo(xOrigo, paddingTop);
                cr.LineTo(xOrigo, paddingTop + yAxisHeight);
                cr.Stroke();
            }

            // Draw y ticks and values
            cr.SetSourceColor(FrtColor);
            cr.LineWidth = 2;
            List<Tick> yTicksList = CalculateTicks(Ymin, Ymax, yTicks);
            foreach (Tick tick in yTicksList)
            {
                float y = yOrigo - tick.Position * yRatio;
                cr.MoveTo(paddingLeft - 5, y);
                cr.RelLineTo(10, 0);

                TextExtents te = cr.TextExtents(tick.Value);
                cr.RelMoveTo(-10 - te.Width - 5, 0.5 * te.Height);
                cr.ShowText(tick.Value);
            }
            cr.Stroke();

            // Draw y label
            if (YLabel != null)
            {
                cr.Save();
                cr.Translate(10, paddingTop + yAxisHeight / 2);
                cr.Rotate(-Math.PI / 2);
                cr.SetFontSize(18);
                TextExtents te = cr.TextExtents(YLabel);
                cr.MoveTo(-0.5 * te.Width, te.Height);
                cr.ShowText(YLabel);
                cr.Restore();
            }

            cr.Translate(xOrigo, yOrigo);

            cr.SetSourceColor(FrtColor);
            switch (Type)
            {
                case PlotType.LinePlot:
                    cr.LineWidth = 2;

                    cr.MoveTo(points[0].X * xRatio, -points[0].Y * yRatio);
                    for (int i = 1; i < points.Length; i++)
                    {
                        double xc = points[i].X * xRatio;
                        double yc = -points[i].Y * yRatio;
                        cr.LineTo(xc, yc);
                    }
                    cr.Stroke();
                    break;
                case PlotType.PointPlot:
                    foreach (Point p in points)
                    {
                        double xc = p.X * xRatio;
                        double yc = -p.Y * yRatio;

                        cr.SetSourceColor(FrtColor);
                        cr.MoveTo(xc, yc);
                        cr.Arc(xc, yc, 2, 0, 2 * Math.PI);
                        cr.Fill();
                    }
                    break;
                case PlotType.PointLinePlot:
                    cr.LineWidth = 2;

                    cr.MoveTo(points[0].X * xRatio, -points[0].Y * yRatio);
                    cr.Arc(points[0].X * xRatio, -points[0].Y * yRatio, 3, 0, 2 * Math.PI);
                    cr.Fill();
                    cr.MoveTo(points[0].X * xRatio, -points[0].Y * yRatio);

                    for (int i = 1; i < points.Length; i++)
                    {
                        double xc = points[i].X * xRatio;
                        double yc = -points[i].Y * yRatio;
                        cr.LineTo(xc, yc);
                        cr.Stroke();
                        cr.Arc(xc, yc, 2, 0, 2 * Math.PI);
                        cr.Fill();
                        cr.MoveTo(xc, yc);
                    }
                    break;
            }

            if (ShowCoordinates)
            {
                foreach (Point p in points)
                {
                    double xc = p.X * xRatio;
                    double yc = -p.Y * yRatio;

                    cr.SetSourceRGB(1, 0, 0);
                    cr.MoveTo(xc, yc);
                    cr.ShowText(String.Format("{0}, {1}", p.X, p.Y));
                }
            }

            // GC from example
            ((IDisposable)cr.GetTarget()).Dispose();
            ((IDisposable)cr).Dispose();
        }

        public void Boundaries(Point[] points, out float xmin, out float xmax, out float ymin, out float ymax)
        {
            xmin = float.NaN;
            xmax = float.NaN;
            ymin = float.NaN;
            ymax = float.NaN;

            foreach (Point p in points)
            {
                xmin = (xmin < p.X) ? xmin : (int)Math.Floor(p.X);
                xmax = (xmax > p.X) ? xmax : (int)Math.Ceiling(p.X);
                ymin = (ymin < p.Y) ? ymin : (int)Math.Floor(p.Y);
                ymax = (ymax > p.Y) ? ymax : (int)Math.Ceiling(p.Y);
            }
        }

        private List<Tick> CalculateTicks(float min, float max, int n)
        {
            List<Tick> ticks = new List<Tick>();
            float stepSize = Math.Abs(max - min) / (n);

            if (min < 0 && max < 0 || min > 0 && max > 0)
            {
                for (float i = min; i <= max; i += stepSize)
                {
                    ticks.Add(new Tick
                    {
                        Position = (stepSize / 2) + i,
                        Value = ((stepSize / 2) + i).ToString("F2")
                    });
                }
            }
            else
            {
                int posTicks = (int)Math.Floor(max / stepSize);
                int negTicks = (int)(Math.Floor(Math.Abs(min) / stepSize));

                for (int i = -negTicks; i <= posTicks; i++)
                {
                    ticks.Add(new Tick
                    {
                        Position = i * stepSize,
                        Value = (i * stepSize).ToString("F2")
                    });
                }
            }

            return ticks;
        }

        private class Tick
        {
            public float Position { get; set; }
            public String Value { get; set; }
        }

        public class Point : IComparable
        {
            private float _x;
            private float _y;

            public Point(float x, float y)
            {
                _x = x;
                _y = y;
            }

            public float X { get { return _x; } }
            public float Y { get { return _y; } }

            public int CompareTo(object obj)
            {
                Point other = (Point)obj;
                if (other.X <= _x)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

    }
}
