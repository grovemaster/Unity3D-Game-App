using Data.Enum;
using Data.Enum.Player;
using ECS.EntityView.Hand;
using Service.Common;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Hand
{
    public class HandService
    {
        public HandPieceEV FindHandPiece(PieceType pieceType, PlayerColor turnPlayer, IEntitiesDB entitiesDB)
        {
            List<HandPieceEV> handPieces = FindAllHandPieces(entitiesDB)
                .Where(hp =>
                    hp.handPiece.PieceType == pieceType
                    && hp.playerOwner.PlayerColor == turnPlayer)
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
                    && hp.playerOwner.PlayerColor == playerOwner)
                .ToList();
        }

        public void DeHighlightHandPieces(List<HandPieceEV> handPieces, IEntitiesDB entitiesDB)
        {
            foreach (HandPieceEV handPiece in handPieces)
            {
                if (handPiece.highlight.IsHighlighted)
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

        private void DeHighlightHandPiece(HandPieceEV handPiece, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                    handPiece.ID,
                    (ref HandPieceEV handPieceToChange) =>
                    {
                        handPieceToChange.highlight.IsHighlighted = false;
                        handPieceToChange.highlight.CurrentColorStates.Clear();
                    });

            handPiece.changeColor.PlayChangeColor = true;
        }
    }
}
