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
    public partial class AutoRunConfig : Window
    {
        
        public AutoRunConfig() : base(WindowType.Toplevel)
        {
            Title = "Auto Sequence Config";
            
            SetSizeRequest(400, 650);
            SetPosition(WindowPosition.Center);
            Console.WriteLine("hello autoconfig");
            ShowAll();
        }
    }
}