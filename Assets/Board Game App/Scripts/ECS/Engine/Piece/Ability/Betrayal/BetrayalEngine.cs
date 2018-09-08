﻿using Data.Enums.Piece;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using Data.Step.Piece.Ability.Betrayal;
using Data.Step.Piece.Ability.ForcedRearrangement;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Piece.Ability.Betrayal
{
    class BetrayalEngine : IStep<BetrayalStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer betrayalSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public BetrayalEngine(ISequencer betrayalSequence)
        {
            this.betrayalSequence = betrayalSequence;
        }

        public void Ready()
        { }

        public void Step(ref BetrayalStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(token.PieceMoved.Location.Location, entitiesDB);

            foreach (PieceEV piece in towerPieces)
            {
                // Only flip enemy pieces that are NOT the commander
                if (piece.PlayerOwner.PlayerColor != currentTurn.TurnPlayer.PlayerColor
                    && piece.Piece.PieceType != PieceType.COMMANDER)
                {
                    FlipPiece(piece, currentTurn.TurnPlayer.PlayerColor);
                }
            }
        }

        private void FlipPiece(PieceEV piece, PlayerColor turnPlayerColor)
        {
            pieceSetService.SetPiecePlayerOwner(piece, turnPlayerColor, entitiesDB);
            pieceSetService.SetPieceSide(
                piece,
                piece.Piece.PieceType == piece.Piece.Front ? PieceSide.BACK : PieceSide.FRONT,
                entitiesDB);
            piece.ChangeColorTrigger.PlayChangeColor = true;
        }

        public void NextAction(ref BetrayalStepState token)
        {
            var forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceCaptured
            };

            betrayalSequence.Next(this, ref forcedRearrangementToken);
        }
    }
}
