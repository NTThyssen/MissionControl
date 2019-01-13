using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Gdk;
using Gtk;
using MissionControl.Data;
using MissionControl.Data.Components;
using Svg;

namespace MissionControl.UI.Widgets
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SVGWidget : Gtk.Bin
    {

        private SvgDocument _svg;
        private ComponentMapping _componentMapping;
        private Dictionary<string, SvgElement> _svgElements;

        private float _svgOriginalW, _svgOriginalH;
        private bool resized;


        public SVGWidget(string filepath, ComponentMapping componentMapping)
        {
            this.Build();

            imageView.SetSizeRequest(600, 600);
            ModifyBg(StateType.Normal, new Gdk.Color(255, 0, 0));
            imageView.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 0));

            _componentMapping = componentMapping;
            _svg = LoadSVG(filepath);
            _svgOriginalW = _svg.Width.Value;
            _svgOriginalH = _svg.Height.Value;
            _svgElements = new Dictionary<string, SvgElement>();
            PopulateElementsDictionary();

        }

        private void PopulateElementsDictionary()
        {
            foreach (Component component in _componentMapping.Components())
            {
                switch (component)
                {
                    case PressureComponent pt:
                    case TemperatureComponent tc:
                        _svgElements.Add(component.GraphicID, _svg.GetElementById(component.GraphicID).Children[0]);
                        ((SvgTextSpan)_svg.GetElementById(component.GraphicID + "_NAME").Children[0]).Text = component.Name;
                        break;
                    case LoadComponent load:
                        break;
                    case TankComponent tank:
                        _svgElements.Add(component.GraphicID, _svg.GetElementById(component.GraphicID).Children[0]);
                        _svgElements.Add(tank.GraphicIDGradient, _svg.GetElementById(tank.GraphicIDGradient));
                        break;
                    case ServoComponent v:
                        _svgElements.Add(component.GraphicID, _svg.GetElementById(component.GraphicID).Children[0]);
                        ((SvgTextSpan)_svg.GetElementById(component.GraphicID + "_NAME").Children[0]).Text = component.Name;
                        break;
                }
            }
        }

        public void Refresh() {
            foreach (Component component in _componentMapping.Components())
            {
                switch (component)
                {
                    case PressureComponent pt:
                        ((SvgTextSpan)_svgElements[component.GraphicID]).Text = String.Format("{0} barR",pt.Relative());
                        break;
                    case TemperatureComponent tc:
                        ((SvgTextSpan)_svgElements[component.GraphicID]).Text = String.Format("{0} °C", tc.Celcius());
                        break;
                    case LoadComponent load:
                        break;
                    case TankComponent tank:

                        float percent = tank.PercentageFull();

                        ((SvgTextSpan)_svgElements[component.GraphicID]).Text = String.Format("{0} %", percent);

                        float gradientStop = 1 - (percent / 100.0f);
                        SvgLinearGradientServer gradient = (SvgLinearGradientServer) _svgElements[tank.GraphicIDGradient];
                        gradient.Stops[0].Offset = 0.0f;
                        gradient.Stops[1].Offset = gradientStop;
                        gradient.Stops[2].Offset = gradientStop;
                        gradient.Stops[3].Offset = 1.0f;
                        break;
                    case ServoComponent v:
                        ((SvgTextSpan)_svgElements[component.GraphicID]).Text = String.Format("{0} %", v.Percentage());
                        break;
                }
            }

            UpdateImage();

        }


        // Inspired by: https://stackoverflow.com/questions/20469706/loading-pixbuf-to-image-widget-in-gtk
        protected override void OnSizeAllocated(Rectangle allocation)
        {
            Console.WriteLine("SizeAllocated {0} {1}", allocation.Width, allocation.Height);
            if (resized)
            {
                resized = false;
                base.OnSizeAllocated(allocation);
            }
            else
            {
                float widthRatio = allocation.Width / _svgOriginalW;
                float heigthRatio = allocation.Height / _svgOriginalH;
                float ratio = Math.Min(widthRatio, heigthRatio);
                float resultWidth = _svgOriginalW * ratio;
                float resultHeight = _svgOriginalH * ratio;

                imageView.Pixbuf = svgPixbuf(resultWidth, resultHeight);
                resized = true;

            }
        }

        private void UpdateImage()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _svg.Draw().Save(ms, ImageFormat.Png);

                imageView.Pixbuf =  new Pixbuf(ms.ToArray()).ScaleSimple((int)_svg.Width.Value, (int)_svg.Height.Value, InterpType.Bilinear);
            }
        }

        private Pixbuf svgPixbuf(float width, float height) 
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _svg.Width = new SvgUnit(SvgUnitType.Pixel, width);
                _svg.Height = new SvgUnit(SvgUnitType.Pixel, height);
                _svg.Draw().Save(ms, ImageFormat.Png);

                return new Pixbuf(ms.ToArray()).ScaleSimple((int)_svg.Width.Value, (int)_svg.Height.Value, InterpType.Bilinear);
            }
       }
     
        public void SaveImage() {
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
            doc.Children.Add(EmbedTextsInGroup(layer1));

            return doc;
        }

        // Search for all texts in document tree
        public SvgGroup EmbedTextsInGroup(SvgGroup root)
        {

            for (int i = 0; i < root.Children.Count; i++)
            {
                SvgElement child = root.Children[i];
                if (child is SvgGroup)
                {
                    EmbedTextsInGroup(child as SvgGroup);
                }
                else if (child is SvgText)
                {
                    root.Children.RemoveAt(i);
                    SvgGroup fixedText = EmbedText(child as SvgText);
                    root.Children.Insert(Math.Min(i, root.Children.Count - 1), fixedText);

                }
            }

            return root;

        }

        // Embed text in group
        public SvgGroup EmbedText(SvgText text)
        {
            float oldX = (text.X.Count > 0) ? text.X[0].Value : 0;
            float oldY = (text.Y.Count > 0) ? text.Y[0].Value : 0;

            text.X = new SvgUnitCollection();
            text.Y = new SvgUnitCollection();

            if (text.HasChildren() && text.Children[0] is SvgTextSpan)
            {
                (text.Children[0] as SvgTextSpan).X = new SvgUnitCollection();
                (text.Children[0] as SvgTextSpan).Y = new SvgUnitCollection();
            }

            SvgGroup group = new SvgGroup();
            group.Transforms.Add(new Svg.Transforms.SvgTranslate(oldX, oldY));
            group.Children.Add(text);

            // Fix weird anchor bug 
            string tempText = ((SvgTextSpan)text.Children[0]).Text;
            return group;
        }


    }
}
