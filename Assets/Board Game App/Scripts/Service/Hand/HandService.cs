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
            List<HandPieceEV> handPieces = CommonService.FindAllEntities<HandPieceEV>(entitiesDB)
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
    }
}
