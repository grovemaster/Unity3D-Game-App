using Data.Check.PreviousMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using Data.Piece.Map;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Check;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Service.Piece.ImmobileCapture
{
    public class ImmobileCaptureService
    {
        private CheckService checkService = new CheckService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        #region No Check Violations
        public bool NoCheckViolationsExist(List<PieceEV> towerPieces, bool immobileCapturePossible, IEntitiesDB entitiesDB)
        {
            if (!immobileCapturePossible)
            {
                return true;
            }

            // EFE scenario, where capturing below one piece is invalid, but capturing the other piece is valid
            int totalNumImmobileCaptures = 0;
            int validNumImmobileCaptures = 0;
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            for (int tierIndex = 1; tierIndex < towerPieces.Count; ++tierIndex)
            {
                if (towerPieces[tierIndex].PlayerOwner.PlayerColor != towerPieces[tierIndex - 1].PlayerOwner.PlayerColor
                    // Also ensure piece that strikes is capable of immobile capture
                    && CanImmobileCapture(currentTurn.TurnPlayer.PlayerColor, towerPieces[tierIndex])
                    && CanImmobileCapture(currentTurn.TurnPlayer.PlayerColor, towerPieces[tierIndex - 1]))
                {
                    totalNumImmobileCaptures++;
                    if (DoesImmobileCaptureResolveOrPreventCheck(towerPieces, tierIndex, entitiesDB))
                    {
                        validNumImmobileCaptures++;
                    }
                }
            }

            return totalNumImmobileCaptures == validNumImmobileCaptures || (totalNumImmobileCaptures > 0 && validNumImmobileCaptures > 0);
        }

        public bool NoTier1CheckViolationsExist(List<PieceEV> towerPieces, IEntitiesDB entitiesDB)
        {
            /**
             * Extra logic required for EFE case, where top E threatens Commander OR capturing bottom E would alter top E's movement to threaten Commander
             * (Commander is not part of tower)
             * 
             * If tower isn't configured as such, no worries
             * If capturing top piece, no worries
             * If capturing bottom piece,
             *      AND top E would threaten Commander as a result, deny bottom capture
             */
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV commander = pieceFindService.FindCommander(currentTurn.TurnPlayer.PlayerColor, entitiesDB);

            return !IsTowerEFEOrFEE(towerPieces, currentTurn.TurnPlayer.PlayerColor)
                || !checkService.DoesLowerTierThreatenCommander(
                    commander, towerPieces[towerPieces.Count - 1], towerPieces[towerPieces.Count - 2], towerPieces, entitiesDB);
        }
        #endregion

        #region Cannot Immobile Capture Ability
        // Return true if piece is not a turn player piece or DOES NOT possess CANNOT_IMMOBILE_CAPTURE ability
        public bool CanImmobileCapture(PlayerColor playerColor, PieceEV piece)
        {
            return piece.PlayerOwner.PlayerColor != playerColor || !AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_IMMOBILE_CAPTURE, piece.Piece.PieceType);
        }
        #endregion

        #region Betrayal Check
        public bool IsFriendlyBetrayalTopOfTower(Vector2 towerLocation, PlayerColor currentTurnColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(towerLocation, entitiesDB);

            return IsFriendlyBetrayalTopOfTower(towerPieces, currentTurnColor, entitiesDB);
        }

        private bool IsFriendlyBetrayalTopOfTower(List<PieceEV> towerPieces, PlayerColor currentTurnColor, IEntitiesDB entitiesDB)
        {
            return towerPieces[towerPieces.Count - 1].PlayerOwner.PlayerColor == currentTurnColor
                && AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, towerPieces[towerPieces.Count - 1].Piece.PieceType);
        }
        #endregion

        private bool IsTowerEFEOrFEE(List<PieceEV> towerPieces, PlayerColor turnPlayerColor)
        {
            return towerPieces.Count == 3
                && ((towerPieces[0].PlayerOwner.PlayerColor != turnPlayerColor
                && towerPieces[1].PlayerOwner.PlayerColor == turnPlayerColor
                && towerPieces[2].PlayerOwner.PlayerColor != turnPlayerColor)
                || (towerPieces[0].PlayerOwner.PlayerColor == turnPlayerColor
                && towerPieces[1].PlayerOwner.PlayerColor != turnPlayerColor
                && towerPieces[2].PlayerOwner.PlayerColor != turnPlayerColor));
        }

        #region Forced Rearrangement
        private bool DoesForcedRearrangementResolveOrPreventCheck(
            PieceEV forcedRearrangementPiece, PieceEV commander, List<PieceEV> towerPieces, IEntitiesDB entitiesDB)
        {
            return checkService.DoesForcedRearrangementResolveOrPreventCheck(forcedRearrangementPiece, commander, towerPieces, entitiesDB);
        }
        #endregion

        #region Save And Restore Previous Move State
        private PreviousTowerState CreatePreviousState(List<PieceEV> towerPieces)
        {
            PreviousTowerState returnValue = new PreviousTowerState
            {
                Pieces = new List<PreviousPieceState>()
            };

            foreach (PieceEV piece in towerPieces)
            {
                returnValue.Pieces.Add(new PreviousPieceState
                {
                    Piece = piece,
                    PlayerColor = piece.PlayerOwner.PlayerColor,
                    PieceType = piece.Piece.PieceType,
                    Location = piece.Location.Location,
                    Tier = piece.Tier.Tier,
                    TopOfTower = piece.Tier.TopOfTower
                });
            }

            return returnValue;
        }

        private void RestorePreviousState(PreviousTowerState previousState, IEntitiesDB entitiesDB)
        {
            foreach (PreviousPieceState state in previousState.Pieces)
            {
                PieceEV piece = state.Piece;
                pieceSetService.SetPiecePlayerOwner(piece, state.PlayerColor, entitiesDB);

                if (piece.Piece.PieceType != state.PieceType)
                {
                    pieceSetService.SetPieceSide(piece, state.PieceType == piece.Piece.Front ? PieceSide.FRONT : PieceSide.BACK, entitiesDB);
                }

                pieceSetService.SetPieceLocationAndTier(piece, state.Location, state.Tier, entitiesDB);
                pieceSetService.SetTopOfTower(piece, entitiesDB, state.TopOfTower);
            }
        }
        #endregion

        private bool DoesImmobileCaptureResolveOrPreventCheck(List<PieceEV> towerPieces, int tierIndex, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            PreviousTowerState previousState = CreatePreviousState(towerPieces);
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV commander = pieceFindService.FindCommander(currentTurn.TurnPlayer.PlayerColor, entitiesDB);

            PieceEV capturedPiece = TempImmobileCapture(
                towerPieces, tierIndex, currentTurn.TurnPlayer.PlayerColor, entitiesDB);

            returnValue = !checkService.IsCommanderInCheck(currentTurn.TurnPlayer.PlayerColor, entitiesDB);

            if (!returnValue && checkService.IsForcedRearrangementPossible(capturedPiece))
            {
                returnValue = DoesForcedRearrangementResolveOrPreventCheck(capturedPiece, commander, towerPieces, entitiesDB);
            }

            RestorePreviousState(previousState, entitiesDB);
            return returnValue;
        }

        private PieceEV TempImmobileCapture(List<PieceEV> towerPieces, int tierIndex, PlayerColor currentTurnColor, IEntitiesDB entitiesDB)
        {
            bool isBetrayalPossible = !IsFriendlyBetrayalTopOfTower(towerPieces, currentTurnColor, entitiesDB);
            PieceEV pieceToStrike = towerPieces[tierIndex].PlayerOwner.PlayerColor == currentTurnColor ? towerPieces[tierIndex] : towerPieces[tierIndex - 1];
            PieceEV pieceToCapture = towerPieces[tierIndex].PlayerOwner.PlayerColor == currentTurnColor ? towerPieces[tierIndex - 1] : towerPieces[tierIndex];

            pieceSetService.SetPieceLocationToHandLocation(pieceToCapture, entitiesDB);
            pieceSetService.SetTopOfTowerToFalse(pieceToCapture, entitiesDB);

            int currentTier = 1;
            foreach (PieceEV towerPiece in towerPieces)
            {
                if (towerPiece.ID.entityID != pieceToCapture.ID.entityID)
                {
                    pieceSetService.SetPieceLocationAndTier(towerPiece, towerPiece.Location.Location, currentTier++, entitiesDB);
                }
            }

            PieceEV topPiece = towerPieces[towerPieces.Count - 1].ID.entityID == pieceToCapture.ID.entityID
                ? towerPieces[towerPieces.Count - 2] : towerPieces[towerPieces.Count - 1];

            isBetrayalPossible = isBetrayalPossible && IsFriendlyBetrayalTopOfTower(towerPieces, currentTurnColor, entitiesDB);

            if (isBetrayalPossible)
            {
                // If friendly betrayal piece becomes topOfTower, by deduction, it's the pieceToStrike
                BetrayalEffectOnTower(towerPieces, currentTurnColor, entitiesDB);
            }

            return pieceToCapture;
        }

        private void BetrayalEffectOnTower(List<PieceEV> towerPieces, PlayerColor betrayalColor, IEntitiesDB entitiesDB)
        {
            foreach (PieceEV piece in towerPieces)
            {
                if (piece.PlayerOwner.PlayerColor != betrayalColor)
                {
                    pieceSetService.SetPiecePlayerOwner(piece, betrayalColor, entitiesDB);
                    pieceSetService.SetPieceSide(piece, piece.Piece.PieceType == piece.Piece.Front ? PieceSide.BACK : PieceSide.FRONT, entitiesDB);
                }
            }
        }
    }
}
