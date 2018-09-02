using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Piece.Ability.Substitution
{
    public struct SubstitutionStepState
    {
        public PieceEV SubstitutionPiece;
        public TileEV TileReferenceEV;
    }
}
