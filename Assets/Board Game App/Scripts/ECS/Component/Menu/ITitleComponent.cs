using Svelto.ECS;
using System;

namespace ECS.Component.Menu
{
    public interface ITitleComponent : IComponent
    {
        DispatchOnSet<bool> Clicked { get; set; }
        Action ClickAction { get; set; }
    }
}
