﻿using Data.Enums;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Turn;
using Scripts.Data.Board;

namespace Service.Board
{
    public class BoardPressService
    {
        public BoardPress DecideAction(
            BoardPressStateInfo stateInfo,
            bool substitutionPossible,
            bool tierExchangePossible,
            TurnEV currentTurn)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = stateInfo.tile.Value;
            int tilePieceId = tileEV.Tile.PieceRefEntityId.GetValueOrDefault();
            // TODO Scenario: Clicked highlighted tile containing opponent piece to initiate mobile capture

            // If hand piece highlighted, it's an attempted drop
            if (stateInfo.handPiece.HasValue)
            {
                returnValue = BoardPress.DROP;
            }
            else if (currentTurn.InitialArrangement.IsInitialArrangementInEffect)
            {   // User is not dropping during Initial Arrangement, so deny
                returnValue = BoardPress.NOTHING;
            }
            else if (substitutionPossible)
            {
                returnValue = BoardPress.SUBSTITUTION;
            }
            else if (tierExchangePossible)
            {
                returnValue = BoardPress.TIER_1_3_EXCHANGE;
            }
            // Tile is clicked, tile highlighted, piece reference exists; move vs mobile capture determined in later engine
            else if (tileEV.Highlight.IsHighlighted
                && stateInfo.piece.HasValue
                && tilePieceId != 0
                && stateInfo.piece.Value.PlayerOwner.PlayerColor == currentTurn.TurnPlayer.PlayerColor)
            {
                returnValue = BoardPress.MOVE_PIECE;
            }
            // NOT move scenario, Piece/Tile clicked, both piece and tile exist, piece reference does not exist
            else if (stateInfo.piece.HasValue && tilePieceId == 0)
            {
                returnValue = BoardPress.CLICK_HIGHLIGHT;
            }

            return returnValue;
        }
    }
}
