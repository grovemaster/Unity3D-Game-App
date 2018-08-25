using Data.Enum.Piece;
using Data.Enum.Piece.Side;
using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface IDropFrontBackComponent : IComponent
    {
        int TileReferenceId { get; set; }
        int HandPieceReferenceId { get; set; }
        PieceType Front { get; set; }
        PieceType Back { get; set; }
        DispatchOnSet<PieceSide> Answer { get; set; } // Engine listens for valid user button press
    }
}
