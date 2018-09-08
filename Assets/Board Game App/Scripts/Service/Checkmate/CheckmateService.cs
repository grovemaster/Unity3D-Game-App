using Data.Constants.Board;
using Data.Enums.Checkmate;
using Data.Enums.Piece.Drop;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using Data.Piece.Map;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Check;
using Service.Drop;
using Service.Hand;
using Service.Piece.Find;
using Service.Piece.ImmobileCapture;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Checkmate
{
    public class CheckmateService
    {
        private CheckService checkService = new CheckService();
        private DestinationTileService destinationTileService = new DestinationTileService();
        private HandService handService = new HandService();
        private ImmobileCaptureService immobileCaptureService = new ImmobileCaptureService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private PreDropService preDropService = new PreDropService();

        #region Public API
        public bool AnyValidMoves(PlayerColor playerColor, IEntitiesDB entitiesDB)
        {
            return AnyValidMoves(playerColor, entitiesDB, DropCheckmateLevel.FIRST_TURN_PLAYER_CHECK);
        }

        public bool DropReleasesCheck(
            PieceEV pieceToDrop,
            Vector2 location,
            PlayerColor playerColor,
            PieceSide side,
            HandPieceEV handPiece,
            IEntitiesDB entitiesDB)
        {
            return DropReleasesCheck(
                pieceToDrop,
                location,
                playerColor,
                side,
                handPiece,
                DropCheckmateLevel.FIRST_TURN_PLAYER_CHECK,
                entitiesDB);
        }
        #endregion

        private bool AnyValidMoves(PlayerColor playerColor, IEntitiesDB entitiesDB, DropCheckmateLevel recursionLevel)
        {
            return AnyValidMobileMoves(playerColor, entitiesDB) || AnyValidImmobileMoves(playerColor, entitiesDB) || AnyValidDrops(playerColor, recursionLevel, entitiesDB);
        }

        private bool AnyValidMobileMoves(PlayerColor playerColor, IEntitiesDB entitiesDB)
        {
            PieceEV[] teamPieces = pieceFindService.FindPiecesByTeam(playerColor, entitiesDB);

            HashSet<Vector2> destinationLocations =
                destinationTileService.CalcDestinationTileLocations(teamPieces, false, entitiesDB);

            return destinationLocations.Count == 0;
        }

        private bool AnyValidImmobileMoves(PlayerColor playerColor, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            foreach (PieceEV piece in allPieces)
            {
                if (piece.Tier.Tier > 1
                    && piece.Tier.TopOfTower // Check each tower only once
                    && ImmobileCapturePossible(piece, playerColor, entitiesDB))
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        private bool AnyValidDrops(PlayerColor playerColor, DropCheckmateLevel recursionLevel, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            List<HandPieceEV> teamHandPieces = handService.FindAllTeamHandPieces(playerColor, entitiesDB).Where(handPiece =>
                handPiece.HandPiece.NumPieces.value > 0).ToList();

            for (int rank = 0; rank < BoardConst.NUM_FILES_RANKS; ++rank)
            {
                for (int file = 0; file < BoardConst.NUM_FILES_RANKS; ++file)
                {
                    foreach (HandPieceEV handPiece in teamHandPieces)
                    {
                        Vector2 location = new Vector2(file, rank);

                        if (DropPossible(handPiece, location, playerColor, recursionLevel, entitiesDB))
                        {
                            returnValue = true;
                            goto ReturnLocation; // Break out of all the for loops
                        }
                    }
                }
            }

            ReturnLocation:
            return returnValue;
        }

        private bool ImmobileCapturePossible(PieceEV piece, PlayerColor playerColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(piece.Location.Location, entitiesDB);

            return immobileCaptureService.ImmobileCapturePossible(towerPieces, playerColor, entitiesDB)
                && immobileCaptureService.NoCheckViolationsExist(towerPieces, true, entitiesDB);
        }

        #region Drop Possible
        private bool DropPossible(
            HandPieceEV handPiece, Vector2 location, PlayerColor playerColor, DropCheckmateLevel recursionLevel, IEntitiesDB entitiesDB)
        {
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(location, entitiesDB);
            bool isFrontValid = preDropService.IsValidFrontDrop(ref handPiece, location, piecesAtLocation, entitiesDB);
            bool isBackValid = preDropService.IsValidBackDrop(ref handPiece, location, piecesAtLocation, entitiesDB);
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, handPiece.HandPiece.PieceType, handPiece.HandPiece.Back, entitiesDB);

            return (isFrontValid && DropReleasesCheck(
                        pieceToDrop,
                        location,
                        playerColor,
                        PieceSide.FRONT,
                        handPiece,
                        recursionLevel,
                        entitiesDB))
                    || (isBackValid && DropReleasesCheck(
                        pieceToDrop,
                        location,
                        playerColor,
                        PieceSide.BACK,
                        handPiece,
                        recursionLevel,
                        entitiesDB));
        }

        private bool DropCheckmateNotViolated(
            PieceEV pieceToDrop,
            Vector2 location,
            PlayerColor playerColor,
            PieceSide side,
            HandPieceEV handPiece,
            DropCheckmateLevel recursionLevel,
            IEntitiesDB entitiesDB)
        {
            bool returnValue = true;
            PlayerColor enemyPlayerColor = TurnService.CalcOtherTurnPlayer(playerColor);

            if (recursionLevel != DropCheckmateLevel.FINAL_TURN_PLAYER_CHECK // Prevent infinite recursion
                && AbilityToPiece.HasAbility(DropAbility.CANNOT_DROP_CHECKMATE, pieceToDrop.Piece.PieceType)
                && checkService.IsCommanderInCheck(enemyPlayerColor, entitiesDB))
            {
                recursionLevel = recursionLevel == DropCheckmateLevel.FIRST_TURN_PLAYER_CHECK
                    ? DropCheckmateLevel.SECOND_ENEMY_PLAYER_CHECK : DropCheckmateLevel.FINAL_TURN_PLAYER_CHECK;
                returnValue = AnyValidMoves(enemyPlayerColor, entitiesDB, recursionLevel);
            }

            return returnValue;
        }

        private bool DropReleasesCheck(
            PieceEV pieceToDrop,
            Vector2 location,
            PlayerColor playerColor,
            PieceSide side,
            HandPieceEV handPiece,
            DropCheckmateLevel recursionLevel,
            IEntitiesDB entitiesDB)
        {
            bool returnValue;

            PieceEV? topPieceAtLocation = pieceFindService.FindTopPieceByLocation(location, entitiesDB);
            pieceSetService.SetTopOfTowerToFalse(topPieceAtLocation, entitiesDB);

            int tier = topPieceAtLocation.HasValue ? topPieceAtLocation.Value.Tier.Tier + 1 : 1;
            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, tier, entitiesDB);
            pieceSetService.SetPiecePlayerOwner(pieceToDrop, playerColor, entitiesDB);
            pieceSetService.SetPieceSide(pieceToDrop, side, entitiesDB);
            handService.DecrementHandPiece(ref handPiece);

            PlayerColor enemyPlayerColor = TurnService.CalcOtherTurnPlayer(playerColor);

            returnValue = DropCheckmateNotViolated(
                pieceToDrop,
                location,
                playerColor,
                side,
                handPiece,
                recursionLevel,
                entitiesDB);

            if (returnValue)
            {
                returnValue = !checkService.IsCommanderInCheck(playerColor, entitiesDB);
            }

            handService.IncrementHandPiece(ref handPiece);
            pieceSetService.SetPieceLocationToHandLocation(pieceToDrop, entitiesDB);

            if (topPieceAtLocation.HasValue)
            {
                pieceSetService.SetTopOfTower(topPieceAtLocation.Value, entitiesDB);
            }

            return returnValue;
        }
        #endregion
    }
}
