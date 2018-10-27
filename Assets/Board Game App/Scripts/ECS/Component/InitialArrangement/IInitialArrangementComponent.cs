namespace ECS.Component.InitialArrangement
{
    public interface IInitialArrangementComponent : IComponent
    {
        bool IsInitialArrangementInEffect { get; set; }
    }
}
