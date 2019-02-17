using System;
using System.Collections;
using System.Collections.Generic;
using MissionControl.Connection.Commands;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public abstract class ComponentMapping
    {
        // ID = similiar to BoardID
        // BC = Byte Count
        public const byte ID_STATE = 200;
        public const byte BC_STATE = 1;

        public const byte ID_TIME = 201;
        public const byte BC_TIME = 4;

        public abstract List<Component> Components();
        public abstract List<MeasuredComponent> MeasuredComponents();
        public abstract List<ComputedComponent> ComputedComponents();
        public abstract List<State> States();

        public State EmergencyState { get; set; }

        public List<ILoggable> Loggables()
        {
            List<ILoggable> loggables = new List<ILoggable>();
            foreach (Component c in Components())
            {
                if (c is ILoggable ic)
                {
                    loggables.Add(ic);
                }
            }
            return loggables;
        }

        public Dictionary<byte, Component> ComponentsByID ()
        {
            Dictionary<byte, Component> dict = new Dictionary<byte, Component>();
            foreach(Component c in Components())
            {
                dict.Add(c.BoardID, c);
            }

            return dict;
        }
    }
}
