namespace ECS.Component.Board.Tile
{
    public interface ITileComponent : IComponent
    {
        int? PieceRefEntityId { get; set; } // Clicked piece may move to this tile
    }
}
