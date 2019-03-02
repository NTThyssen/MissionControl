using System;
using Gtk;
using Pango;


namespace MissionControl.UI.Widgets
{
    public class DSectionTitle : Label
    {
        public DSectionTitle(string title)
        {
            Text = title;
            Xalign = 0;

            ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
            ModifyFont(new FontDescription
            {
                Size = Convert.ToInt32(20 * Pango.Scale.PangoScale)
            });

        }
        
        public DSectionTitle(string title, int fontSize)
        {
            Text = title;
            Xalign = 0;

            ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));
            ModifyFont(new FontDescription
            {
                Size = Convert.ToInt32(fontSize * Pango.Scale.PangoScale)
            });

        }
    }
}
