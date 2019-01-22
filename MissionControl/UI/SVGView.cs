using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cairo;
using Gdk;
using Gtk;
using MissionControl.Data;
using MissionControl.Data.Components;
using Svg;
using Svg.Transforms;

namespace MissionControl.UI.Widgets
{
    public class SVGView : DrawingArea
    {

        private SvgDocument _svg;
        private Session _session;
        private Dictionary<string, SvgElement> _svgElements;
        private List<SvgText> _svgTexts;

        private float _svgOriginalW, _svgOriginalH;

        bool _didRefresh;



        public SVGView(string filepath, ref Session session)
        {

            _session = session;

            _svgElements = new Dictionary<string, SvgElement>();
            _svgTexts = new List<SvgText>();

            _svg = LoadSVG(filepath);
            _svgOriginalW = 900;
            _svgOriginalH = 717.54936f;

            PopulateElementsDictionary();

            SetSizeRequest((int)_svgOriginalW, (int)_svgOriginalH);
            ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            ExposeEvent += (o, args) => {
                UpdateImage();
                Console.WriteLine("SVG ExposeEvent!");
            };

          


        }

        private void PopulateElementsDictionary()
        {
            foreach (Component component in _session.Mapping.Components())
            {
                switch (component)
                {
                    case VoltageComponent vc:
                    case PressureComponent pt:
                    case TemperatureComponent tc:
                    case LoadComponent load:
                        _svgElements.Add(component.GraphicID, FindTextByID(component.GraphicID));
                        FindTextByID(component.GraphicID + "_NAME").Text = component.Name;
                        break;
                    case TankComponent tank:
                        _svgElements.Add(component.GraphicID, FindTextByID(component.GraphicID));
                        _svgElements.Add(tank.GraphicIDGradient, _svg.GetElementById(tank.GraphicIDGradient));
                        FindTextByID(component.GraphicID + "_NAME").Text = component.Name;
                        break;
                    case ValveComponent v:
                        _svgElements.Add(component.GraphicID, FindTextByID(component.GraphicID));
                        _svgElements.Add(v.GraphicIDSymbol, _svg.GetElementById(v.GraphicIDSymbol));
                        FindTextByID(component.GraphicID + "_NAME").Text = component.Name;
                        break;
                }
            }
        }

        public void MarkValve(ValveComponent component)
        {
            _svgElements[component.GraphicIDSymbol].Stroke = new SvgColourServer(System.Drawing.Color.FromArgb(255, 0, 0));
            _didRefresh = true;
            UpdateImage();
        }

        public void UnmarkValve(ValveComponent component)
        {
            _svgElements[component.GraphicIDSymbol].Stroke = new SvgColourServer(System.Drawing.Color.FromArgb(255, 255, 255));
            _didRefresh = true;
            UpdateImage();
        }

        public void Refresh()
        {
            foreach (Component component in _session.Mapping.Components())
            {
                switch (component)
                {
                    case PressureComponent pt:
                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} barR", pt.Relative());
                        break;
                    case TemperatureComponent tc:
                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} °C", tc.Celcius());
                        break;
                    case LoadComponent load:
                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} N", load.Newtons());
                        break;
                    case TankComponent tank:

                        float percent = tank.PercentageFull();

                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} %", percent);

                        float gradientStop = 1 - (percent / 100.0f);
                        SvgLinearGradientServer gradient = (SvgLinearGradientServer)_svgElements[tank.GraphicIDGradient];
                        gradient.Stops[0].Offset = 0.0f;
                        gradient.Stops[1].Offset = gradientStop;
                        gradient.Stops[2].Offset = gradientStop;
                        gradient.Stops[3].Offset = 1.0f;
                        break;
                    case ServoComponent servo:
                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} %", servo.Percentage());
                        break;
                    case SolenoidComponent solenoid:
                        ((SvgText)_svgElements[component.GraphicID]).Text = solenoid.State().ToString();
                        break;
                    case VoltageComponent battery:
                        ((SvgText)_svgElements[component.GraphicID]).Text = string.Format("{0} V / {1} %", battery.Volts(), Math.Floor(battery.Percentage() * 100) / 100);
                        break;
                }
            }
            _didRefresh = true;
            UpdateImage();

        }


        // Inspired by: https://stackoverflow.com/questions/20469706/loading-pixbuf-to-image-widget-in-gtk
        /*protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            if (_didRefresh) { _didRefresh = false; return; }

            if (resized)
            {
                resized = false;
                base.OnSizeAllocated(allocation);
            }
            else
            {
                Console.WriteLine("SVG Widget Redrawn form Size Allocate {0} {1}", allocation.Width, allocation.Height);
                float widthRatio = allocation.Width / _svgOriginalW;
                float heigthRatio = allocation.Height / _svgOriginalH;
                float ratio = heigthRatio;
                int resultWidth = (int)(_svgOriginalW * ratio);
                int resultHeight = (int)(_svgOriginalH * ratio);

                UpdateImage(resultWidth, resultHeight);
                //UpdateImage();
                resized = true;

            }
        }*/

        private void UpdateImage()
        {

            int width = Allocation.Width;
            int height = Allocation.Height;
            Context cr = Gdk.CairoHelper.Create(this.GdkWindow);

            cr.SetSourceRGB(0, 0, 0);
            cr.Rectangle(0, 0, width, height);
            cr.Fill();

            cr.Save();
            using (MemoryStream ms = new MemoryStream())
            {
                _svg.Draw(width,height).Save(ms, ImageFormat.Png);
    //            imageView.Pixbuf = new Pixbuf(ms.ToArray());
                CairoHelper.SetSourcePixbuf(cr, new Pixbuf(ms.ToArray()), 0,0);
            }

            cr.Paint();
            cr.Restore();

            cr.SetSourceRGB(1, 1, 1);
            foreach (SvgText text in _svgTexts)
            {
                if (text.Text == null) continue;

                float dx = (text.X[0].Value / _svgOriginalW) * width;
                float dy = (text.Y[0].Value / _svgOriginalH) * height;
                cr.MoveTo(dx, dy);


                if (text.FontSize.Value is float size)
                {
                    cr.SetFontSize(size);
                }

                FontWeight weight = (text.FontWeight == SvgFontWeight.Bold) ? FontWeight.Bold : FontWeight.Normal;
                FontSlant slant = (text.FontStyle == SvgFontStyle.Italic) ? FontSlant.Italic : FontSlant.Normal;
                cr.SelectFontFace("sans serif", slant, weight);

                if (text.TextAnchor == SvgTextAnchor.Middle)
                {
                    TextExtents te = cr.TextExtents(text.Text);
                    cr.RelMoveTo(-te.Width / 2.0, 0);
                }

                cr.ShowText(text.Text);
            }

   
            // GC from example
            ((IDisposable)cr.GetTarget()).Dispose();
            ((IDisposable)cr).Dispose();
        }

        public void SaveImage()
        {
            MemoryStream ms = new MemoryStream();
            _svg.Draw().Save(ms, ImageFormat.Png);
            _svg.Write(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources/test.svg"));
            using (FileStream file = new FileStream(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources/test.png"), FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] bytes = ms.ToArray();
                file.Write(bytes, 0, bytes.Length);
                ms.Close();
            }
        }




        /*
         * Due to a bug in the SVG library, we have to embed all texts in a group a place the group at the texts place.
         * Not doing thing will result in the text now being show.
         */

        // Loads the svg document and fixes text placement bug
        public SvgDocument LoadSVG(string filename)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            SvgDocument doc = SvgDocument.Open(path);

            SvgGroup layer1 = doc.GetElementById("layer1") as SvgGroup;
            doc.Children.Remove(layer1);
            doc.Children.Add(CutAllTextElements(layer1, 0, 0));

            return doc;
        }

        // Search for all texts in document tree
        public SvgGroup CutAllTextElements(SvgGroup root, float tx, float ty)
        {
            List<SvgElement> texts = new List<SvgElement>();
            SvgTranslate trans = root.Transforms[0] as SvgTranslate;
            if (trans != null)
            {
                tx += trans.X;
                ty += trans.Y;
            }

            for (int i = 0; i < root.Children.Count; i++)
            {
                SvgElement child = root.Children[i];
                if (child is SvgGroup)
                {
                    CutAllTextElements(child as SvgGroup, tx, ty);
                }
                else if (child is SvgText)
                {
                    SvgText text = child as SvgText;
                    if (text.Children[0] is SvgTextSpan span)
                    {
                        text.Text = span.Text;
                        text.TextAnchor = span.TextAnchor;
                        text.FontSize = span.FontSize;
                        text.FontWeight = span.FontWeight;
                    }

                    text.Children.Clear();
                    text.X[0] = new SvgUnit(text.X[0].Value + tx);
                    text.Y[0] = new SvgUnit(text.Y[0].Value + ty);
                    _svgTexts.Add(text);
                    texts.Add(child);

                }
            }

            foreach (SvgElement se in texts)
            {
                root.Children.Remove(se);
            }
            return root;

        }

        public SvgText FindTextByID(string id)
        {
            return _svgTexts.Find((SvgText obj) => obj.ID.Equals(id));
        }

    }

}
