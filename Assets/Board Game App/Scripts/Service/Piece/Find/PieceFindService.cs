using Data.Constants.Board;
using Data.Enum.Piece;
using Data.Enum.Player;
using ECS.EntityView.Piece;
using Service.Common;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Piece.Find
{
    public class PieceFindService
    {
        public PieceEV FindPieceEV(int entityId, IEntitiesDB entitiesDB)
        {
            return CommonService.FindEntity<PieceEV>(entityId, entitiesDB);
        }

        public PieceEV[] FindAllBoardPieces(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece => piece.Location.Location != BoardConst.HAND_LOCATION).ToArray();
        }

        public PieceEV? FindPieceEVById(int? entityId, IEntitiesDB entitiesDB)
        {
            PieceEV returnValue = CommonService.FindEntityById<PieceEV>(entityId, entitiesDB);

            return returnValue.ID.entityID != 0 ? (PieceEV?)returnValue : null;
        }

        public PieceEV? FindTopPieceByLocation(Vector2 location, IEntitiesDB entitiesDB)
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

        public List<PieceEV> FindPiecesByLocation(Vector2 location, IEntitiesDB entitiesDB)
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
        public PieceEV[] FindPiecesByTeam(PlayerColor team, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB)
                .Where(piece => piece.PlayerOwner.PlayerColor == team && piece.Tier.TopOfTower).ToArray();
        }

        public PieceEV FindFirstPieceByLocationAndType(
            Vector2 location, PieceType pieceType, IEntitiesDB entitiesDB)
        {
            List<PieceEV> piecesInHands = CommonService.FindAllEntities<PieceEV>(entitiesDB)
                .Where(piece =>
                    piece.Location.Location == location
                    && (piece.Piece.Front == pieceType || piece.Piece.Back == pieceType)
                ).ToList();

            if (piecesInHands.Count == 0)
            {
                throw new InvalidOperationException("There must exist a piece at specified location of specified type");
            }

            return piecesInHands[0];
        }

        public List<PieceEV> FindPiecesByTypeAndFile(PieceType pieceType, float file, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB).Where(piece =>
                piece.Piece.PieceType == pieceType && piece.Location.Location.x == file)
                .ToList();
        }

        public PieceEV FindCommander(PlayerColor commanderColor, IEntitiesDB entitiesDB)
        {
            return FindAllBoardPieces(entitiesDB).First(piece =>
                piece.Piece.PieceType == PieceType.COMMANDER && piece.PlayerOwner.PlayerColor == commanderColor);
        }
    }
}
