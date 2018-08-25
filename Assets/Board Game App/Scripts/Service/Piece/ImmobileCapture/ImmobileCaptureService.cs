using Data.Constants.Board;
using Data.Enum.Player;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Check;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System;
using System.Collections.Generic;

namespace Service.Piece.ImmobileCapture
{
    public class ImmobileCaptureService
    {
        private CheckService checkService = new CheckService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TurnService turnService = new TurnService();

        public bool NoCheckViolationsExist(List<PieceEV> towerPieces, bool immobileCapturePossible, IEntitiesDB entitiesDB)
        {
            if (!immobileCapturePossible)
            {
                return true;
            }

            bool returnValue = true;

            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV commander = pieceFindService.FindCommander(currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            int numCommanderThreats = checkService.CalcNumCommanderThreats(commander.PlayerOwner.PlayerColor, entitiesDB);

            if (commander.Location.Location == towerPieces[0].Location.Location)
            {
                if (currentTurn.Check.CommanderInCheck)
                {
                    if (IsAdjacentPieceEnemy(commander, towerPieces))
                    {
                        if (commander.Tier.TopOfTower)
                        {
                            returnValue = numCommanderThreats == 1;
                        }
                        else
                        {
                            returnValue = !WouldCommanderBeTopOfTower(commander, towerPieces)
                                || !WouldCommanderBeInCheck(commander, towerPieces, entitiesDB);
                        }
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }
            else
            {
                PieceEV topPiece = towerPieces[towerPieces.Count - 1];

                if (currentTurn.Check.CommanderInCheck)
                {
                    if (topPiece.PlayerOwner.PlayerColor == commander.PlayerOwner.PlayerColor)
                    {
                        returnValue = false;
                    }
                    else
                    {
                        if (numCommanderThreats > 1 || !checkService.DoesTopOfTowerThreatenCommander(commander, topPiece, entitiesDB))
                        {
                            returnValue = false;
                        }
                        else
                        {
                            // FEE scenario, enemy topOfTower would have new movement, potentially releasing Commander from check
                            if (towerPieces[towerPieces.Count - 2].PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor)
                            {
                                returnValue = !checkService.DoesLowerTierThreatenCommander(commander, topPiece, entitiesDB);
                            }
                        }
                    }
                }
                else
                {
                    // FEE scenario, enemy topOfTower would have new movement, potentially putting Commander into check
                    if (topPiece.PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor
                        && towerPieces[towerPieces.Count - 2].PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor)
                    {
                        returnValue = !checkService.DoesLowerTierThreatenCommander(commander, topPiece, entitiesDB);
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
                || !checkService.DoesLowerTierThreatenCommander(commander, towerPieces[towerPieces.Count - 1], entitiesDB);
        }

        // TODO Move to utility service/class and call from there
        private bool IsAdjacentPieceEnemy(PieceEV pieceToCompare, List<PieceEV> towerPieces)
        {
            switch (pieceToCompare.Tier.Tier)
            {
                case 1:
                case 3:
                    return pieceToCompare.PlayerOwner.PlayerColor != towerPieces[1].PlayerOwner.PlayerColor;
                case 2:
                    return pieceToCompare.PlayerOwner.PlayerColor != towerPieces[0].PlayerOwner.PlayerColor
                        || (towerPieces.Count > 2 && pieceToCompare.PlayerOwner.PlayerColor != towerPieces[2].PlayerOwner.PlayerColor);
                default:
                    throw new InvalidOperationException("Invalid tier number");
            }
        }

        private bool WouldCommanderBeTopOfTower(PieceEV commander, List<PieceEV> towerPieces)
        {
            return commander.Tier.Tier == towerPieces.Count - 1;
        }

        private bool WouldCommanderBeInCheck(PieceEV commander, List<PieceEV> towerPieces, IEntitiesDB entitiesDB)
        {
            PieceEV formerTopOfTower = towerPieces[towerPieces.Count - 1];
            formerTopOfTower.Location.Location = BoardConst.HAND_LOCATION;
            commander.Tier.TopOfTower = true;

            bool returnValue = checkService.IsCommanderInCheck(commander.PlayerOwner.PlayerColor, entitiesDB);

            commander.Tier.TopOfTower = false;
            formerTopOfTower.Location.Location = commander.Location.Location;

            return returnValue;
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
    }
}
