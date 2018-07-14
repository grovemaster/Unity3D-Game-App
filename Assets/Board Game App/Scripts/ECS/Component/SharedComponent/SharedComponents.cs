using Data.Enum;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Component.SharedComponent
{
    public interface ILocation: IComponent
    {
        Vector3 Location { get; set; }
    }

    public interface IHighlight: IComponent
    {
        DispatchOnSet<bool> IsPressed { get; set; } // Impl sets IsPressed = true, Engine does logic, then sets IsPressed = false
        bool IsHighlighted { get; set; }
        DispatchOnSet<HighlightState> CurrentColor { get; set; } // Engine sets value, Impl makes actual change
        // TODO Eventually will need a HighlightAnimationComponent specifically for color change effects
    }
}
