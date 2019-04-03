using System;
using GLib;

namespace MissionControl.Data.Components
{
    public class StackHealthComponent : MeasuredComponent, ILoggable
    {
        public StackHealthComponent(byte boardID, string graphicID2, string graphicID3, string graphicID4, string name) : base(boardID, null, name)
        {
            GraphicId2 = graphicID2;
            GraphicId3 = graphicID3;
            GraphicId4 = graphicID4;
        }


        public override string TypeName => "Health";
        public override string ToDisplay()
        {
            throw new NotImplementedException();
        }
        public string GraphicId2 { get;  }
        public string GraphicId3 { get;  }
        public string GraphicId4 { get;  }
        public bool IsMainAlive { get; private set; }
        public bool IsSensorAlive { get; private set; }
        public bool IsActuatorAlive { get; private set; }
        public override int ByteSize => 1;
        public override bool Signed { get; }
        public override int Raw { get; }
        public override void Set(int val)
        {
            IsMainAlive = (val  & 1) == 1 ;
            IsActuatorAlive = (val & 2) > 0;
            IsSensorAlive = (val & 4) > 0; 
         }

        public string ToLog()
        {
           return Name + "Hp";
        }

        public string LogHeader()
        {
            return "Health";
        }
    }
}