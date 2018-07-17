using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using Svelto.ECS;

namespace ECS.EntityView.Board.Tile
{
    public struct TileEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public ITile tile;
        public ILocation location;
        public IHighlight highlight;
        public IChangeColorComponent changeColorComponent;
    }
}
