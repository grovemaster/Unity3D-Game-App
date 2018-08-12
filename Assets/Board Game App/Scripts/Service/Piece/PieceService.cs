using Data.Constants.Board;
using Data.Enum;
using Data.Enum.Player;
using Data.Piece;
using Data.Piece.Back.Gold;
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
                case PieceType.GOLD:
                    return new GoldData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }

        public static PieceEV[] FindAllBoardPieces(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece => piece.Location.Location != BoardConst.HAND_LOCATION).ToArray();
        }

        public static PieceEV? FindPieceEVById(int? entityId, IEntitiesDB entitiesDB)
        {
            PieceEV returnValue = CommonService.FindEntityById<PieceEV>(entityId, entitiesDB);

            return returnValue.ID.entityID != 0 ? (PieceEV?)returnValue : null;
        }

        public static PieceEV? FindTopPieceByLocation(Vector2 location, IEntitiesDB entitiesDB)
        {
            PieceEV? returnValue = null;
            PieceEV[] pieces = FindAllBoardPieces(entitiesDB);

            for (int i = 0; i < pieces.Length; ++i)
            {
                if (pieces[i].Location.Location == location && pieces[i].Tier.TopOfTower)
                {
                    returnValue = pieces[i];
                    break;
                }
            }

            return returnValue;
        }

        public static List<PieceEV> FindPiecesByLocation(Vector2 location, IEntitiesDB entitiesDB)
        {
            List<PieceEV> returnValue = new List<PieceEV>();
            PieceEV[] pieces = FindAllBoardPieces(entitiesDB);

            for (int i = 0; i < pieces.Length; ++i)
            {
                if (pieces[i].Location.Location == location)
                {
                    returnValue.Add(pieces[i]);
                }
            }

            returnValue.Sort(delegate (PieceEV p1, PieceEV p2)
            { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });

            return returnValue;
        }

        /**
         * Only finds pieces at top of tower
         */
        public static PieceEV[] FindPiecesByTeam(PlayerColor team, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB)
                .Where(piece => piece.PlayerOwner.PlayerColor == team && piece.Tier.TopOfTower).ToArray();
        }

        public static PieceEV FindFirstPieceByLocationAndType(
            Vector2 location, PieceType pieceType, IEntitiesDB entitiesDB)
        {
            List<PieceEV> piecesInHands = CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece =>
                    piece.Location.Location == location
                    && piece.Piece.PieceType == pieceType
                ).ToList();

            if (piecesInHands.Count == 0)
            {
                throw new InvalidOperationException("There must exist a piece at specified location of specified type");
            }

            return piecesInHands[0];
        }

        public static List<PieceEV> FindPiecesByTypeAndFile(PieceType pieceType, float file, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB).Where(piece =>
                piece.Piece.PieceType == pieceType && piece.Location.Location.x == file)
                .ToList();
        }

        public static void SetPieceLocationToHandLocation(PieceEV pieceEV, IEntitiesDB entitiesDB)
        {
            SetPieceLocationAndTier(pieceEV, BoardConst.HAND_LOCATION, 0, entitiesDB);
        }

        public static void SetPieceLocationAndTier(PieceEV pieceEV, Vector2 location, int tier, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Tier.TopOfTower = true;
                    pieceToChange.Tier.Tier = tier;
                    pieceToChange.Location.Location = location;
                });
        }

        public static void SetTopOfTower(PieceEV pieceEV, IEntitiesDB entitiesDB, bool topOfTower = true)
        {
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Tier.TopOfTower = topOfTower;
                });
        }

        public static void SetPiecePlayerOwner(PieceEV pieceEV, PlayerColor playerOwner,  IEntitiesDB entitiesDB)
        {
            Direction newDirection = DirectionService.CalcDirection(playerOwner);
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.PlayerOwner.PlayerColor = playerOwner;
                    pieceToChange.Piece.Direction = newDirection;
                });
        }

        public static void SetTopOfTowerToFalse(PieceEV? pieceEV, IEntitiesDB entitiesDB)
        {
            if (!pieceEV.HasValue)
            {
                return;
            }

            entitiesDB.ExecuteOnEntity(
                pieceEV.Value.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Tier.TopOfTower = false;
                });
        }
    }
}
