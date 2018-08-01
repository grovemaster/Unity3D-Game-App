namespace ECS.Component.Piece.Tower
{
    public interface ITowerTierComponent : IComponent
    {
        bool TopOfTower { get; set; }
        int Tier { get; set; }
    }
}
