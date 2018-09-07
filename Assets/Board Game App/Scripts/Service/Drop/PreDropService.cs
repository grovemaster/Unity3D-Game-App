using Data.Enums.Piece;
using Data.Enums.Piece.Drop;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using Data.Piece.Map;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Piece.Factory;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Drop
{
    public class PreDropService
    {
        private PieceFactory pieceFactory = new PieceFactory();
        private PieceFindService pieceFindService = new PieceFindService();
        private TurnService turnService = new TurnService();

        // TODO Should not care about all pieces at location, just top piece at location, fix that
        public bool IsValidFrontDrop(
            ref HandPieceEV handPiece, Vector2 location, List<PieceEV> piecesAtLocation, IEntitiesDB entitiesDB)
        {
            return IsValidDrop(ref handPiece, ref location, piecesAtLocation, PieceSide.FRONT, entitiesDB);
        }

        public bool IsValidBackDrop(
            ref HandPieceEV handPiece, Vector2 location, List<PieceEV> piecesAtLocation, IEntitiesDB entitiesDB)
        {
            return IsValidDrop(ref handPiece, ref location, piecesAtLocation, PieceSide.BACK, entitiesDB);
        }

        public bool IsValidForcedRearrangementDrop(List<PieceEV> piecesAtLocation, IEntitiesDB entitiesDB)
        {
            // TODO Rearrange service logic later to make this a more robust function
            return IsEmptyTile(piecesAtLocation) || IsValidEarthLinkDrop(piecesAtLocation, PieceSide.FRONT) || IsValidEarthLinkDrop(piecesAtLocation, null);
        }

        private bool IsValidDrop(
            ref HandPieceEV handPiece, ref Vector2 location, List<PieceEV> piecesAtLocation, PieceSide side, IEntitiesDB entitiesDB)
        {
            return (IsEmptyTile(piecesAtLocation) || IsValidEarthLinkDrop(piecesAtLocation, side) || IsValidEarthLinkDrop(piecesAtLocation, null))
                && DoesNotViolateDoubleFileDrop(ref handPiece, ref location, side, entitiesDB)
                && DoesNotViolateTerritoryDrop(ref handPiece, ref location, side, entitiesDB)
                && DoesNotViolateForcedRearrangementFrontDrop(side, entitiesDB);
        }

        private bool IsEmptyTile(List<PieceEV> piecesAtLocation)
        {
            return piecesAtLocation.Count == 0;
        }

        #region Forced Rearrangement
        private bool DoesNotViolateForcedRearrangementFrontDrop(PieceSide side, IEntitiesDB entitiesDB)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            return currentTurn.ForcedRearrangementStatus.ForcedRearrangmentActive && side == PieceSide.BACK ? false : true;
        }
        #endregion

        #region Double File Drop (Pawn & Bronze)
        private bool DoesNotViolateDoubleFileDrop(
            ref HandPieceEV handPiece, ref Vector2 location, PieceSide side, IEntitiesDB entitiesDB)
        {
            return !HasDropAbility(ref handPiece, side, DropAbility.DOUBLE_FILE_DROP)
                || NoOtherSameTypesInFile(ref handPiece, ref location, side, entitiesDB);
        }

        private bool NoOtherSameTypesInFile(ref HandPieceEV handPiece, ref Vector2 location, PieceSide side, IEntitiesDB entitiesDB)
        {
            PlayerColor playerColor = handPiece.PlayerOwner.PlayerColor;

            return pieceFindService.FindPiecesByTypeAndFile(
                GetPieceType(ref handPiece, side),
                location.x,
                entitiesDB
                ).Where(piece => piece.PlayerOwner.PlayerColor == playerColor).ToList()
                .Count == 0;
        }
        #endregion

        #region Earth-Link
        private bool IsValidEarthLinkDrop(List<PieceEV> piecesAtLocation, PieceSide? side)
        {
            // Since this is called after IsEmptyTile with an OR condition, we know there is at least one piece on the tile
            PieceEV topPiece = piecesAtLocation[piecesAtLocation.Count - 1];

            return topPiece.Tier.Tier < 3 && AbilityToPiece.HasAbility(GetEarthLinkAbilityType(side), topPiece.Piece.PieceType);
        }

        private DropAbility GetEarthLinkAbilityType(PieceSide? side)
        {
            switch (side)
            {
                case PieceSide.FRONT:
                    return DropAbility.EARTH_LINK_FRONT;
                case PieceSide.BACK:
                    return DropAbility.EARTH_LINK_BACK;
                default:
                    return DropAbility.EARTH_LINK;
            }
        }
        #endregion

        #region Territory Drop
        private bool DoesNotViolateTerritoryDrop(
            ref HandPieceEV handPiece, ref Vector2 location, PieceSide side, IEntitiesDB entitiesDB)
        {
            TurnEV turnPlayer = turnService.GetCurrentTurnEV(entitiesDB);

            return !HasDropAbility(ref handPiece, side, DropAbility.TERRITORY_DROP)
                || turnService.IsRankWithinTerritory(turnPlayer, location.y);
        }
        #endregion

        #region Utility
        private bool HasDropAbility(ref HandPieceEV handPiece, PieceSide side, DropAbility dropAbility)
        {
            List<DropAbility> drop = pieceFactory.CreateIPieceData(GetPieceType(ref handPiece, side)).Abilities.Drop;

            return drop.Contains(dropAbility);
        }

        private PieceType GetPieceType(ref HandPieceEV handPiece, PieceSide side)
        {
            return side == PieceSide.FRONT ? handPiece.HandPiece.PieceType : handPiece.HandPiece.Back;
        }
        #endregion
    }
}
