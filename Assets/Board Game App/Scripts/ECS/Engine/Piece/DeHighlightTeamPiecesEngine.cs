using Data.Enums;
using Data.Enums.Player;
using Data.Step;
using Data.Step.Hand;
using Data.Step.Modal;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Board.Tile.Highlight;
using Service.Hand;
using Service.Piece.Find;
using Service.Piece.Highlight;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Piece
{
    class DeHighlightTeamPiecesEngine :
        IStep<HandPiecePressStepState>, IStep<PressStepState>, IStep<CancelModalStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceHighlightService pieceHighlightService = new PieceHighlightService();
        private TileHighlightService tileHighlightService = new TileHighlightService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            // Purpose of this engine is to ensure only a single piece per team is highlighted at any given time
            // If the goal is to UN-highlight a piece, there is no need to check for other highlighted pieces
            if (token.PiecePressState == PiecePressState.UNCLICKED)
            {
                return;
            }

            PieceEV piece = pieceFindService.FindPieceEV(token.PieceEntityId, entitiesDB);
            DeHighlightPlayerPiecesAndTiles(piece);
            UnHighlightHandPieces();
        }

        public void Step(ref HandPiecePressStepState token, int condition)
        {
            HandPieceEV handPieceToChange = handService.FindHandPiece(token.HandPieceEntityId, entitiesDB);

            List<PieceEV> alteredPieces = pieceHighlightService.DeHighlightPlayerPieces(
                handPieceToChange.PlayerOwner.PlayerColor, entitiesDB);

            if (alteredPieces.Count > 0)
            {
                tileHighlightService.DeHighlightOtherTeamTilePieces(
                    alteredPieces, handPieceToChange.PlayerOwner.PlayerColor, entitiesDB);
            }

            List<HandPieceEV> otherHandPieces = handService.FindAllTeamHandPiecesExcept(
                token.HandPieceEntityId, handPieceToChange.PlayerOwner.PlayerColor, entitiesDB);
            handService.DeHighlightHandPieces(otherHandPieces, entitiesDB);
        }

        public void Step(ref CancelModalStepState token, int condition)
        {
            PlayerColor turnPlayer = turnService.GetCurrentTurnEV(entitiesDB).TurnPlayer.PlayerColor;

            List<PieceEV> alteredPieces = pieceHighlightService.DeHighlightPlayerPieces(turnPlayer, entitiesDB);
            tileHighlightService.DeHighlightOtherTeamTilePieces(alteredPieces, turnPlayer, entitiesDB);
            UnHighlightHandPieces();
        }

        private void DeHighlightPlayerPiecesAndTiles(PieceEV pieceToNotChange)
        {
            List<PieceEV> alteredPieces = pieceHighlightService.DeHighlightOtherPlayerPieces(
                pieceToNotChange.ID.entityID, pieceToNotChange.PlayerOwner.PlayerColor, entitiesDB);

            if (alteredPieces.Count > 0)
            {
                tileHighlightService.DeHighlightOtherTeamTilePieces(
                    alteredPieces, pieceToNotChange.PlayerOwner.PlayerColor, entitiesDB);
            }
        }

        private void UnHighlightHandPieces()
        {
            handService.UnHighlightAllHandPieces(entitiesDB);
        }
    }
}
