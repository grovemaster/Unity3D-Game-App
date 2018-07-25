using Data.Enum;
using Data.Step;
using Data.Step.Hand;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Board.Tile.Highlight;
using Service.Hand;
using Service.Piece;
using Service.Piece.Highlight;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Piece
{
    class DeHighlightTeamPiecesEngine : IStep<HandPiecePressStepState>, IStep<PressStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private PieceHighlightService pieceHighlightService = new PieceHighlightService();
        private TileHighlightService tileHighlightService = new TileHighlightService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            // Purpose of this engine is to ensure only a single piece per team is highlighted at any given time
            // If the goal is to UN-highlight a piece, there is no need to check for other highlighted pieces
            if (token.piecePressState == PiecePressState.UNCLICKED)
            {
                return;
            }

            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);
            DeHighlightPlayerPiecesAndTiles(piece);
            UnHighlightHandPieces();
        }

        public void Step(ref HandPiecePressStepState token, int condition)
        {
            HandPieceEV handPieceToChange = handService.FindHandPiece(token.handPieceEntityId, entitiesDB);

            List<PieceEV> alteredPieces = pieceHighlightService.DeHighlightPlayerPieces(
                handPieceToChange.playerOwner.PlayerColor, entitiesDB);

            if (alteredPieces.Count > 0)
            {
                tileHighlightService.DeHighlightOtherTeamTilePieces(
                    alteredPieces, handPieceToChange.playerOwner.PlayerColor, entitiesDB);
            }

            List<HandPieceEV> otherHandPieces = handService.FindAllTeamHandPiecesExcept(
                token.handPieceEntityId, handPieceToChange.playerOwner.PlayerColor, entitiesDB);
            handService.DeHighlightHandPieces(otherHandPieces, entitiesDB);
        }

        private void DeHighlightPlayerPiecesAndTiles(PieceEV pieceToNotChange)
        {
            List<PieceEV> alteredPieces = pieceHighlightService.DeHighlightOtherPlayerPieces(
                pieceToNotChange.ID.entityID, pieceToNotChange.playerOwner.PlayerColor, entitiesDB);

            if (alteredPieces.Count > 0)
            {
                tileHighlightService.DeHighlightOtherTeamTilePieces(
                    alteredPieces, pieceToNotChange.playerOwner.PlayerColor, entitiesDB);
            }
        }

        private void UnHighlightHandPieces()
        {
            handService.UnHighlightAllHandPieces(entitiesDB);
        }
    }
}
