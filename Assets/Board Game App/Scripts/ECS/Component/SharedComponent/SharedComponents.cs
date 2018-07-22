using Data.Enum;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Component.SharedComponent
{
    public interface ILocationComponent : IComponent
    {
        Vector3 Location { get; set; }
    }

    public interface IHighlightComponent : IComponent
    {
        DispatchOnSet<bool> IsPressed { get; set; } // Impl sets IsPressed = true, Engine does logic, then sets IsPressed = false
        bool IsHighlighted { get; set; }
        HashSet<HighlightState> CurrentColorStates { get; set; } // Engine sets values, Impl makes actual change in animation component
    }
}
