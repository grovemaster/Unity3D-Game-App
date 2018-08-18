using Data.Enum;
using Data.Enum.Player;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Common;
using Service.Turn;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Hand
{
    public class HandService
    {
        private TurnService turnService = new TurnService();

        public HandPieceEV FindHandPiece(PieceType pieceType, PlayerColor turnPlayer, IEntitiesDB entitiesDB)
        {
            List<HandPieceEV> handPieces = FindAllHandPieces(entitiesDB)
                .Where(hp =>
                    hp.HandPiece.PieceType == pieceType
                    && hp.PlayerOwner.PlayerColor == turnPlayer)
                .ToList();

            if (handPieces.Count > 1  || handPieces.Count == 0)
            {
                throw new InvalidOperationException("Expected number of hand pieces not found");
            }

            return handPieces[0];
        }

        public HandPieceEV FindHandPiece(int handPieceEntityId, IEntitiesDB entitiesDB)
        {
            return CommonService.FindEntity<HandPieceEV>(handPieceEntityId, entitiesDB);
        }

        public List<HandPieceEV> FindAllTeamHandPiecesExcept(int handPieceToExcludeId, PlayerColor playerOwner, IEntitiesDB entitiesDB)
        {
            return FindAllHandPieces(entitiesDB)
                .Where(hp =>
                    hp.ID.entityID != handPieceToExcludeId
                    && hp.PlayerOwner.PlayerColor == playerOwner)
                .ToList();
        }

        public void DeHighlightHandPieces(List<HandPieceEV> handPieces, IEntitiesDB entitiesDB)
        {
            foreach (HandPieceEV handPiece in handPieces)
            {
                if (handPiece.Highlight.IsHighlighted)
                {
                    DeHighlightHandPiece(handPiece, entitiesDB);
                }
            }
        }

        public void UnHighlightAllHandPieces(IEntitiesDB entitiesDB)
        {
            List<HandPieceEV> handPieces = FindAllHandPieces(entitiesDB)
                .OfType<HandPieceEV>().ToList();
            DeHighlightHandPieces(handPieces, entitiesDB);
        }

        public HandPieceEV[] FindAllHandPieces(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<HandPieceEV>(entitiesDB);
        }

        public HandPieceEV? FindHighlightedHandPiece(IEntitiesDB entitiesDB)
        {
            List<HandPieceEV> handPieces = FindAllHandPieces(entitiesDB)
                .Where(hp => hp.Highlight.IsHighlighted)
                .ToList();

            return handPieces.Count > 0 ? (HandPieceEV?)handPieces[0] : null;
        }

        public void AddPieceToHand(PieceEV pieceToCapture, IEntitiesDB entitiesDB, PlayerColor? handOwner = null)
        {
            if (!handOwner.HasValue)
            {
                handOwner = turnService.GetCurrentTurnEV(entitiesDB).TurnPlayer.PlayerColor;
            }

            HandPieceEV handHoldingCapturedPiece = FindHandPiece(
                pieceToCapture.Piece.PieceType, handOwner.Value, entitiesDB);
            handHoldingCapturedPiece.HandPiece.NumPieces.value++;
        }

        public void DecrementHandPiece(ref HandPieceEV handPiece)
        {
            handPiece.HandPiece.NumPieces.value--;
        }

        private void DeHighlightHandPiece(HandPieceEV handPiece, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                    handPiece.ID,
                    (ref HandPieceEV handPieceToChange) =>
                    {
                        handPieceToChange.Highlight.IsHighlighted = false;
                        handPieceToChange.Highlight.CurrentColorStates.Clear();
                    });

            handPiece.ChangeColorTrigger.PlayChangeColor = true;
        }
    }
}
