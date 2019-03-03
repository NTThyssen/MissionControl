using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
        private float _svgOriginalRatio;

        SvgColourServer nominalColor = new SvgColourServer(System.Drawing.Color.FromArgb(255, 255, 255));
        SvgColourServer warningColor = new SvgColourServer(System.Drawing.Color.FromArgb(255, 0, 0));
        SvgColourServer disabledColor = new SvgColourServer(System.Drawing.Color.FromArgb(150, 150, 150));


        Cairo.Color cNominalColor = new Cairo.Color(1, 1, 1);
        Cairo.Color cWarningColor = new Cairo.Color(1, 0, 0);
        Cairo.Color cDisabledColor = new Cairo.Color(150 / 255.0, 150 / 255.0, 150 / 255.0);

        public SVGView(string filepath, ref Session session)
        {

            _session = session;
           
            _svgElements = new Dictionary<string, SvgElement>();
            _svgTexts = new List<SvgText>();

            _svg = LoadSVG(filepath);
            _svgOriginalW = 900;
            _svgOriginalH = 717.54936f;
            _svgOriginalRatio = _svgOriginalW / _svgOriginalH;

            PopulateElementsDictionary();

            SetSizeRequest((int) (_svgOriginalW * 0.75), (int) (_svgOriginalH * 0.75));
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
                    case FlowComponent flow:
                        _svgElements.Add(component.GraphicID, FindTextByID(component.GraphicID));
                        FindTextByID(component.GraphicID + "_NAME").Text = component.Name;
                        break;
                    case LevelComponent level:
                        _svgElements.Add(component.GraphicID, FindTextByID(component.GraphicID));
                        _svgElements.Add(level.GraphicIDGradient, _svg.GetElementById(level.GraphicIDGradient));
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
            //_svgElements[component.GraphicIDSymbol].Stroke = new SvgColourServer(System.Drawing.Color.FromArgb(255, 0, 0));
            //UpdateImage();
        }

        public void UnmarkValve(ValveComponent component)
        {
            //_svgElements[component.GraphicIDSymbol].Stroke = new SvgColourServer(System.Drawing.Color.FromArgb(255, 255, 255));
            //UpdateImage();
        }

        public void Refresh()
        {
            foreach (Component component in _session.Mapping.Components())
            {
                SvgText text = (SvgText)_svgElements[component.GraphicID];

                if (!component.Enabled)
                {
                    FindTextByID(component.GraphicID + "_NAME").Color = disabledColor;
                    text.Text = "N/A";
                    text.Color = disabledColor;
                    continue;
                }
                else
                {
                    FindTextByID(component.GraphicID + "_NAME").Color = nominalColor;
                }

                switch (component)
                {
                    case PressureComponent pt:
                        if (_session.Setting.ShowAbsolutePressure.Value)
                        {
                            text.Text = Component.ToRounded(pt.Absolute(_session.Setting.TodayPressure.Value), 2) + " bara";
                            text.Color = pt.IsNominal(pt.Absolute(_session.Setting.TodayPressure.Value)) ? nominalColor : warningColor;
                        }
                        else
                        {
                            text.Text = Component.ToRounded(pt.Relative(), 2) + " barg";
                            text.Color = pt.IsNominal(pt.Relative()) ? nominalColor : warningColor;
                        }
                        break;
                    case TemperatureComponent tc:
                        text.Text = tc.ToDisplay() + " °C";
                        text.Color = tc.IsNominal(tc.Celcius()) ? nominalColor : warningColor;
                        break;
                    case LoadComponent load:
                        text.Text = load.ToDisplay() + " N";
                        text.Color = load.IsNominal(load.Newtons()) ? nominalColor : warningColor;
                        break;
                    case LevelComponent level:
                        text.Text = level.ToDisplay() + " %";

                        float levelGradientStop = 1 - (level.PercentageFull() / 100.0f);
                        SvgLinearGradientServer levelGradient = (SvgLinearGradientServer)_svgElements[level.GraphicIDGradient];
                        levelGradient.Stops[0].Offset = 0.0f;
                        levelGradient.Stops[1].Offset = levelGradientStop;
                        levelGradient.Stops[2].Offset = levelGradientStop;
                        levelGradient.Stops[3].Offset = 1.0f;
                        break;
                    case TankComponent tank:
                        text.Text = tank.ToDisplay();

                        float gradientStop = 1 - (tank.ToPercentage() / 100.0f);
                        SvgLinearGradientServer gradient = (SvgLinearGradientServer)_svgElements[tank.GraphicIDGradient];
                        gradient.Stops[0].Offset = 0.0f;
                        gradient.Stops[1].Offset = gradientStop;
                        gradient.Stops[2].Offset = gradientStop;
                        gradient.Stops[3].Offset = 1.0f;
                        break;
                    case ServoComponent servo:
                        text.Text = servo.ToDisplay() + " %";
                        break;
                    case SolenoidComponent solenoid:
                        text.Text = solenoid.State().ToString();
                        break;
                    case VoltageComponent voltage:
                        float volts = voltage.Volts();
                        text.Text = string.Format(CultureInfo.InvariantCulture, "{0} V / {1} %", volts, Math.Floor(voltage.Percentage() * 100) / 100);
                        text.Color = voltage.IsNominal(voltage.Volts()) ? nominalColor : warningColor;
                        break;
                    case FlowComponent flow:
                        text.Text = flow.ToDisplay() + " kg/s";
                        text.Color = flow.IsNominal(flow.MassFlow) ? nominalColor : warningColor;
                        break;
                }
            }
            SvgText ofRatioText = FindTextByID("MASS_FLOW_RATIO");
            FlowComponent fuelFlow = _session.Mapping.ComponentsByID()[100] as FlowComponent;
            FlowComponent oxidFlow = _session.Mapping.ComponentsByID()[101] as FlowComponent;
            if (ofRatioText != null && fuelFlow != null && oxidFlow != null)
            {
                float ofRatio = (oxidFlow.MassFlow / fuelFlow.MassFlow);
                ofRatioText.Text = !float.IsNaN(ofRatio) ? 
                        Component.ToRounded(oxidFlow.MassFlow / fuelFlow.MassFlow, 2) 
                        : "0.00";
            }
            
            UpdateImage();

        }

        private void UpdateImage()
        {
            float scale = Math.Min(Allocation.Width / _svgOriginalW, Allocation.Height / _svgOriginalH);
            int width = (int) (_svgOriginalW * scale);
            int height =(int) (_svgOriginalH * scale);
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

                float dx = text.X[0].Value / _svgOriginalW * width;
                float dy = text.Y[0].Value / _svgOriginalH * height;
                cr.MoveTo(dx, dy);

                if (text.Color == warningColor) 
                {
                    cr.SetSourceColor(cWarningColor);
                } 
                else if (text.Color == disabledColor)
                {
                    cr.SetSourceColor(cDisabledColor);
                }
                else {
                    cr.SetSourceColor(cNominalColor);
                }

                if (text.FontSize.Value is float size)
                {
                    cr.SetFontSize(size * scale);
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
