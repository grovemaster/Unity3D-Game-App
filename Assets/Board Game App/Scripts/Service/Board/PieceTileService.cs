using System;
using System.Collections.Generic;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Player;
using Data.Piece.Map;
using Data.Step.Board;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Scripts.Data.Board;
using Service.Board.Tile;
using Service.Check;
using Service.Hand;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;

namespace Service.Board
{
    public class PieceTileService
    {
        private CheckService checkService = new CheckService();
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TileService tileService = new TileService();
        private TurnService turnService = new TurnService();

        public BoardPressStateInfo FindBoardPressStateInfo(IEntitiesDB entitiesDB, ref BoardPressStepState token)
        {
            BoardPressStateInfo returnValue = new BoardPressStateInfo
            {
                piece = pieceFindService.FindPieceEVById(token.PieceEntityId, entitiesDB),
                tile = tileService.FindTileEVById(token.TileEntityId, entitiesDB),
                pieceAtDestination = null, // If movement-related information is later required
                handPiece = handService.FindHighlightedHandPiece(entitiesDB)
            };

            if (!returnValue.piece.HasValue
                && returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId != 0) // Find by tile information
            {
                returnValue.piece = pieceFindService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            if (!returnValue.tile.HasValue) // Find by piece information
            {
                returnValue.tile = tileService.FindTileEV(
                    returnValue.piece.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue && !returnValue.piece.HasValue)
            {
                returnValue.piece = pieceFindService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue)
            {
                returnValue.pieceAtDestination = pieceFindService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            // Movement: Piece is clicked with intention to move a DIFFERENT piece to that tile location
            if (returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.Value != 0
                && returnValue.piece.HasValue
                && returnValue.piece.Value.Location.Location == returnValue.tile.Value.Location.Location)
            {
                returnValue.piece = pieceFindService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            return returnValue;
        }

        public bool IsSubstitutionPossible(BoardPressStateInfo stateInfo, TurnEV currentTurn, IEntitiesDB entitiesDB)
        {
            return !currentTurn.InitialArrangement.IsInitialArrangementInEffect
                && checkService.IsSubstitutionPossible(stateInfo.piece, stateInfo.tile, entitiesDB);
        }

        public bool IsTierExchangePossible(BoardPressStateInfo stateInfo, TurnEV currentTurn, IEntitiesDB entitiesDB)
        {
            if (!stateInfo.tile.HasValue || currentTurn.InitialArrangement.IsInitialArrangementInEffect)
            {
                return false;
            }

            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(stateInfo.tile.Value.Location.Location, entitiesDB);

            return IsTier1ExchangePossible(towerPieces, currentTurn.TurnPlayer.PlayerColor)
                || IsTier3ExchangePossible(towerPieces, currentTurn.TurnPlayer.PlayerColor);
        }

        private bool IsTier1ExchangePossible(List<PieceEV> towerPieces, PlayerColor currentTurnColor)
        {
            return towerPieces.Count == 3
                && AbilityToPiece.HasAbility(OtherMoveAbility.TIER_1_EXCHANGE, towerPieces[0].Piece.PieceType)
                && !AbilityToPiece.HasAbility(OtherMoveAbility.TIER_3_EXCHANGE, towerPieces[2].Piece.PieceType)
                && towerPieces[2].PlayerOwner.PlayerColor == currentTurnColor
                && towerPieces[0].PlayerOwner.PlayerColor == currentTurnColor;
        }

        private bool IsTier3ExchangePossible(List<PieceEV> towerPieces, PlayerColor currentTurnColor)
        {
            return towerPieces.Count == 3
                && AbilityToPiece.HasAbility(OtherMoveAbility.TIER_3_EXCHANGE, towerPieces[2].Piece.PieceType)
                && towerPieces[2].PlayerOwner.PlayerColor == currentTurnColor
                && towerPieces[0].PlayerOwner.PlayerColor == currentTurnColor
                && !AbilityToPiece.HasAbility(OtherMoveAbility.CANNOT_TIER_3_EXCHANGE, towerPieces[0].Piece.PieceType)
                && !AbilityToPiece.HasAbility(OtherMoveAbility.TIER_1_EXCHANGE, towerPieces[0].Piece.PieceType);
        }
    }
}
