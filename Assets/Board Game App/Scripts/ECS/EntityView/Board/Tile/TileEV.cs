using ECS.Component.Board.Tile;
using ECS.Component.SharedComponent;
using Svelto.ECS;

namespace ECS.EntityView.Board.Tile
{
    public struct TileEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public ITileComponent Tile;
        public ILocationComponent Location;
        public IHighlightComponent Highlight;
        public IChangeColorComponent ChangeColorTrigger;
    }
}
