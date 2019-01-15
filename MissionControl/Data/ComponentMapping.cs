using System;
using System.Collections;
using System.Collections.Generic;
using MissionControl.Connection.Commands;
using MissionControl.Data.Components;

namespace MissionControl.Data
{
    public abstract class ComponentMapping
    {
        public abstract List<Component> Components();
        public abstract List<StateCommand> States();

    }
}
