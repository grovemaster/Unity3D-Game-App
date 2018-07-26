using Data.Constants.Board;
using Data.Enum;
using Data.Enum.Player;
using Data.Piece;
using Data.Piece.Front.Pawn;
using ECS.EntityView.Piece;
using Service.Common;
using Service.Directions;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Piece
{
    public static class PieceService
    {
        public static PieceEV FindPieceEV(int entityId, IEntitiesDB entitiesDB)
        {
            return CommonService.FindEntity<PieceEV>(entityId, entitiesDB);
        }

        public static IPieceData CreateIPieceData(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.PAWN:
                    return new PawnData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }

        public static PieceEV[] FindAllBoardPieces(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece => piece.location.Location != BoardConst.HAND_LOCATION).ToArray();
        }

        public static PieceEV? FindPieceEVById(int? entityId, IEntitiesDB entitiesDB)
        {
            PieceEV returnValue = CommonService.FindEntityById<PieceEV>(entityId, entitiesDB);

            return returnValue.ID.entityID != 0 ? (PieceEV?)returnValue : null;
        }

        // TODO This will become a list once towers are enabled
        public static PieceEV? FindPieceByLocation(Vector3 location, IEntitiesDB entitiesDB)
        {
            PieceEV? returnValue = null;

            PieceEV[] pieces = FindAllBoardPieces(entitiesDB);

            for (int i = 0; i < pieces.Length; ++i)
            {
                // Tile always on z=0, pieces always on z>=1
                if (pieces[i].location.Location.x == location.x
                    && pieces[i].location.Location.y == location.y)
                {
                    returnValue = pieces[i];
                    break;
                }
            }

            return returnValue;
        }

        public static PieceEV[] FindPiecesByTeam(PlayerColor team, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB)
                .Where(piece => piece.playerOwner.PlayerColor == team).ToArray();
        }

        public static PieceEV FindFirstPieceByLocationAndType(
            Vector3 location, PieceType pieceType, IEntitiesDB entitiesDB)
        {
            List<PieceEV> piecesInHands = CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece =>
                    piece.location.Location == location
                    && piece.piece.PieceType == pieceType
                ).ToList();

            if (piecesInHands.Count == 0)
            {
                throw new InvalidOperationException("There must exist a piece in hand of specified type");
            }

            return piecesInHands[0];
        }

        public static void SetPieceLocationToHandLocation(PieceEV pieceEV, IEntitiesDB entitiesDB)
        {
            SetPieceLocation(pieceEV, BoardConst.HAND_LOCATION, entitiesDB);
        }

        public static void SetPieceLocation(PieceEV pieceEV, Vector3 location, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToHand) =>
                {
                    pieceToHand.location.Location = location;
                });
        }

        public static void SetPiecePlayerOwner(PieceEV pieceEV, PlayerColor playerOwner,  IEntitiesDB entitiesDB)
        {
            Direction newDirection = DirectionService.CalcDirection(playerOwner);
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToHand) =>
                {
                    pieceToHand.playerOwner.PlayerColor = playerOwner;
                    pieceToHand.piece.Direction = newDirection;
                });
        }
    }
}
