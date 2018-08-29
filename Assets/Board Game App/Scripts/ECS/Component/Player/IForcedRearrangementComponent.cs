namespace ECS.Component.Player
{
    public interface IForcedRearrangementComponent : IComponent
    {
        bool ForcedRearrangmentActive { get; set; }
}
}
