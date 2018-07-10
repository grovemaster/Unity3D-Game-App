namespace ECS.Component.Board.Tile
{
    class ITile : IComponent
    {
        public int? pieceRefEntityId; // Clicked piece may move to this tile
    }
}
