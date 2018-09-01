using ECS.EntityView.Piece;

namespace Data.Step.Piece.Capture
{
    public struct ImmobileCapturePieceStepState
    {
        public PieceEV pieceToStrike;
        public PieceEV PieceToCapture;
        public bool BetrayalPossible;
    }
}
