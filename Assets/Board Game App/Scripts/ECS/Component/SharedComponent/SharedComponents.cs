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
        DispatchOnSet<bool> IsHighlighted { get; set; } // Impl sets IsHighlighted, Engine does logic
        DispatchOnSet<HighlightState> CurrentColor { get; set; } // Engine sets value, Impl makes actual change
    }
}
