using Data.Check.PreviousMove;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Check;
using Service.Piece.Find;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Move
{
    public class DetermineMoveTypeService
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private CheckService checkService = new CheckService();
        private PieceFindService pieceFindService = new PieceFindService();

        public bool IsMobileStackValid(PieceEV pieceToCalc, Vector2 destination, IEntitiesDB entitiesDB)
        {
            return IsMobileMoveValid(pieceToCalc, destination, true, entitiesDB);
        }

        public bool IsMobileCaptureValid(PieceEV pieceToCalc, Vector2 destination, IEntitiesDB entitiesDB)
        {
            return IsMobileMoveValid(pieceToCalc, destination, false, entitiesDB);
        }

        // Will need to be careful of betrayal effect when doing temp move
        private bool IsMobileMoveValid(PieceEV pieceToCalc, Vector2 destination, bool shouldStackEnemyPiece, IEntitiesDB entitiesDB)
        {
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();
            // Make temp move while saving old info
            PreviousMoveState previousMoveState = destinationTileService.SaveCurrentMove(pieceToCalc, destination, allPieces);
            PreviousTowerState? previousDestinationTowerState = previousMoveState.pieceCaptured.HasValue
                && destinationTileService.BetrayalInEffect(previousMoveState.pieceToMove.Piece)
                ? destinationTileService.SaveDestinationTowerState(previousMoveState, allPieces) : null;
            destinationTileService.MakeTemporaryMove(pieceToCalc, destination, allPieces, shouldStackEnemyPiece);

            bool returnValue = !checkService.IsCommanderInCheck(pieceToCalc.PlayerOwner.PlayerColor, entitiesDB);

            destinationTileService.RestorePreviousState(previousMoveState, previousDestinationTowerState);

            return returnValue;
        }
    }
}
