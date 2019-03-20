using System;
using GLib;

namespace MissionControl.Data.Components
{
    public class StackHealthComponent : MeasuredComponent, ILoggable
    {
        public StackHealthComponent(byte boardID, string graphicID2, string graphicID3, string graphicID4, string name) : base(boardID, null, name)
        {
            this.graphicID2 = graphicID2;
            this.graphicID3 = graphicID3;
            this.graphicID4 = graphicID4;
        }


        public override string TypeName => "Health";
        public override string ToDisplay()
        {
            throw new NotImplementedException();
        }
        public string graphicID2 { get;  }
        public string graphicID3 { get;  }
        public string graphicID4 { get;  }
        public override int ByteSize => 1;
        public override bool Signed { get; }
        public override int Raw { get; }
        public override void Set(int val)
        {
            throw new NotImplementedException();
        }

        public string ToLog()
        {
           return Name + "Health";
        }

        public string LogHeader()
        {
            throw new NotImplementedException();
        }
    }
}