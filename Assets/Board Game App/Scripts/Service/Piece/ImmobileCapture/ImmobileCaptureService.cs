using Data.Check.PreviousMove;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Check;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;

namespace Service.Piece.ImmobileCapture
{
    public class ImmobileCaptureService
    {
        private CheckService checkService = new CheckService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        public bool NoCheckViolationsExist(List<PieceEV> towerPieces, bool immobileCapturePossible, IEntitiesDB entitiesDB)
        {
            if (!immobileCapturePossible)
            {
                return true;
            }

            bool returnValue = true;

            for (int tierIndex = 1; tierIndex < towerPieces.Count; ++tierIndex)
            {
                if (towerPieces[tierIndex].PlayerOwner.PlayerColor != towerPieces[tierIndex - 1].PlayerOwner.PlayerColor)
                {
                    if (!DoesImmobileCaptureResolveOrPreventCheck(towerPieces, tierIndex, entitiesDB))
                    {
                        returnValue = false;
                        break;
                    }
                }
            }

            return returnValue;
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
        private PreviousImmobileCaptureState CreatePreviousState(List<PieceEV> towerPieces)
        {
            PreviousImmobileCaptureState returnValue = new PreviousImmobileCaptureState
            {
                pieces = new List<PreviousPieceState>()
            };

            foreach (PieceEV piece in towerPieces)
            {
                returnValue.pieces.Add(new PreviousPieceState
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

        private void RestorePreviousState(PreviousImmobileCaptureState previousState, IEntitiesDB entitiesDB)
        {
            foreach (PreviousPieceState state in previousState.pieces)
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
            PreviousImmobileCaptureState previousState = CreatePreviousState(towerPieces);
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

            return pieceToCapture;
        }
    }
}
