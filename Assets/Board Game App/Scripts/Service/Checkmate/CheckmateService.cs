using Data.Constants.Board;
using Data.Enums.Piece.Side;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Check;
using Service.Drop;
using Service.Hand;
using Service.Piece.Find;
using Service.Piece.ImmobileCapture;
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
        private PreDropService preDropService = new PreDropService();

        public bool AnyValidMoves(TurnEV turn, IEntitiesDB entitiesDB)
        {
            return AnyValidMobileMoves(turn, entitiesDB) || AnyValidImmobileMoves(turn, entitiesDB) || AnyValidDrops(turn, entitiesDB);
        }

        private bool AnyValidMobileMoves(TurnEV turn, IEntitiesDB entitiesDB)
        {
            PieceEV[] teamPieces = pieceFindService.FindPiecesByTeam(turn.TurnPlayer.PlayerColor, entitiesDB);

            HashSet<Vector2> destinationLocations =
                destinationTileService.CalcDestinationTileLocations(teamPieces, false, entitiesDB);

            return destinationLocations.Count == 0;
        }

        private bool AnyValidImmobileMoves(TurnEV turn, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            foreach (PieceEV piece in allPieces)
            {
                if (piece.Tier.Tier > 1
                    && piece.Tier.TopOfTower // Check each tower only once
                    && ImmobileCapturePossible(piece, turn, entitiesDB))
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        private bool AnyValidDrops(TurnEV turn, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            List<HandPieceEV> teamHandPieces = handService.FindAllTeamHandPieces(turn.TurnPlayer.PlayerColor, entitiesDB).Where(handPiece =>
                handPiece.HandPiece.NumPieces.value > 0).ToList();

            for (int rank = 0; rank < BoardConst.NUM_FILES_RANKS; ++rank)
            {
                for (int file = 0; file < BoardConst.NUM_FILES_RANKS; ++file)
                {
                    foreach (HandPieceEV handPiece in teamHandPieces)
                    {
                        Vector2 location = new Vector2(file, rank);

                        if (DropPossible(handPiece, location, turn, entitiesDB))
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

        private bool ImmobileCapturePossible(PieceEV piece, TurnEV turn, IEntitiesDB entitiesDB)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(piece.Location.Location, entitiesDB);

            return immobileCaptureService.ImmobileCapturePossible(towerPieces, turn, entitiesDB)
                && immobileCaptureService.NoCheckViolationsExist(towerPieces, true, entitiesDB);
        }

        private bool DropPossible(HandPieceEV handPiece, Vector2 location, TurnEV currentTurn, IEntitiesDB entitiesDB)
        {
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(location, entitiesDB);
            bool isFrontValid = preDropService.IsValidFrontDrop(ref handPiece, location, piecesAtLocation, entitiesDB);
            bool isBackValid = preDropService.IsValidBackDrop(ref handPiece, location, piecesAtLocation, entitiesDB);
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, handPiece.HandPiece.PieceType, handPiece.HandPiece.Back, entitiesDB);

            return (isFrontValid && checkService.DropReleasesCheck(
                        pieceToDrop,
                        location,
                        currentTurn,
                        PieceSide.FRONT,
                        entitiesDB))
                    || (isBackValid && checkService.DropReleasesCheck(
                        pieceToDrop,
                        location,
                        currentTurn,
                        PieceSide.BACK,
                        entitiesDB));
        }
    }
}
