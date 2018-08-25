using Data.Constants.Board;
using Data.Enum;
using Data.Enum.Piece.Side;
using Data.Enum.Player;
using ECS.EntityView.Piece;
using Service.Directions;
using Svelto.ECS;
using UnityEngine;

namespace Service.Piece.Set
{
    public class PieceSetService
    {
        public void SetPieceLocationToHandLocation(PieceEV pieceEV, IEntitiesDB entitiesDB)
        {
            SetPieceLocationAndTier(pieceEV, BoardConst.HAND_LOCATION, 0, entitiesDB);
        }

        public void SetPieceLocationAndTier(PieceEV pieceEV, Vector2 location, int tier, IEntitiesDB entitiesDB)
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

        public void SetPieceSide(PieceEV pieceEV, PieceSide side, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Piece.PieceType = side == PieceSide.FRONT ? pieceToChange.Piece.Front : pieceToChange.Piece.Back;
                });
        }

        public void SetTopOfTower(PieceEV pieceEV, IEntitiesDB entitiesDB, bool topOfTower = true)
        {
            entitiesDB.ExecuteOnEntity(
                pieceEV.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Tier.TopOfTower = topOfTower;
                });
        }

        public void SetPiecePlayerOwner(PieceEV pieceEV, PlayerColor playerOwner, IEntitiesDB entitiesDB)
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

        public void SetTopOfTowerToFalse(PieceEV? pieceEV, IEntitiesDB entitiesDB)
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
