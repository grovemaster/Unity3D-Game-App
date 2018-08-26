using System;
using System.Collections.Generic;
using Data.Enum.AB;
using Data.Enum.Piece.Drop;
using Data.Enum.Piece.Side;
using Data.Step.Drop;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Piece.Factory;
using Service.Piece.Find;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.Drop
{
    class PreDropAbilitiesEngine : IStep<DropPrepStepState>, IQueryingEntitiesEngine
    {
        private PieceFactory pieceFactory = new PieceFactory();
        private PieceFindService pieceFindService = new PieceFindService();

        private readonly ISequencer dropSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PreDropAbilitiesEngine(ISequencer dropSequence)
        {
            this.dropSequence = dropSequence;
        }

        public void Ready()
        { }

        public void Step(ref DropPrepStepState token, int condition)
        {
            bool isFrontValid = IsValidFrontDrop(ref token);
            bool isBackValid = IsValidBackDrop(ref token);

            if (isFrontValid && isBackValid)
            {
                NextActionDropModal(ref token);
            }
            else if (isFrontValid ^ isBackValid)
            {
                NextActionDrop(ref token, isFrontValid ? PieceSide.FRONT : PieceSide.BACK);
            }
        }

        private bool IsValidFrontDrop(ref DropPrepStepState token)
        {
            return (IsEmptyTile(ref token) || IsValidEarthLinkFrontDrop(ref token)) && DoesNotViolateDoublePawnDrop(ref token);
        }

        private bool IsValidBackDrop(ref DropPrepStepState token)
        {
            return IsEmptyTile(ref token) || IsValidEarthLinBackDrop(ref token);
        }

        private bool IsEmptyTile(ref DropPrepStepState token)
        {
            return pieceFindService.FindPiecesByLocation(token.DestinationTile.Location.Location, entitiesDB)
                .Count == 0;
        }

        #region Double Pawn Drop
        private bool DoesNotViolateDoublePawnDrop(ref DropPrepStepState token)
        {
            return !HasDoublePawnDrop(token.HandPiece) || NoOtherPawnsInFile(ref token);
        }

        private bool HasDoublePawnDrop(HandPieceEV handPiece)
        {
            DropAbility? dropAbility = pieceFactory.CreateIPieceData(handPiece.HandPiece.PieceType).Abilities.Drop;
            return dropAbility.HasValue && dropAbility.Value == DropAbility.DOUBLE_PAWN_DROP;
        }

        private bool NoOtherPawnsInFile(ref DropPrepStepState token)
        {
            return pieceFindService.FindPiecesByTypeAndFile(
                token.HandPiece.HandPiece.PieceType,
                token.DestinationTile.Location.Location.x,
                entitiesDB
                ).Count == 0;
        }
        #endregion

        #region Earth-Link
        private bool IsValidEarthLinkFrontDrop(ref DropPrepStepState token)
        {
            return IsValidEarthLinkDrop(ref token, PieceSide.FRONT);
        }

        private bool IsValidEarthLinBackDrop(ref DropPrepStepState token)
        {
            return IsValidEarthLinkDrop(ref token, PieceSide.BACK);
        }

        private bool IsValidEarthLinkDrop(ref DropPrepStepState token, PieceSide? side)
        {
            // Since this is called after IsEmptyTile with an OR condition, we know there is at least one piece on the tile
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(token.DestinationTile.Location.Location, entitiesDB);
            PieceEV topPiece = piecesAtLocation[piecesAtLocation.Count - 1];
            DropAbility? topPieceDropAbility = pieceFactory.CreateIPieceData(topPiece.Piece.PieceType).Abilities.Drop;
            bool hasEarthLinkAbility = topPieceDropAbility.HasValue && topPieceDropAbility.Value == GetEarthLinkAbilityType(side);

            return topPiece.Tier.Tier < 3 && hasEarthLinkAbility;
        }

        private DropAbility GetEarthLinkAbilityType(PieceSide? side)
        {
            switch (side)
            {
                case PieceSide.FRONT:
                    return DropAbility.EARTH_LINK_FRONT;
                case PieceSide.BACK:
                    return DropAbility.EARTH_LINK_BACK;
                case null:
                    return DropAbility.EARTH_LINK_BACK;
                default:
                    throw new InvalidOperationException("Invalid or unsupported PieceSide");
            }
        }
        #endregion

        #region NextAction
        private void NextActionDropModal(ref DropPrepStepState token)
        {
            dropSequence.Next(this, ref token, (int)StepAB.A);
        }

        private void NextActionDrop(ref DropPrepStepState token, PieceSide pieceSide)
        {
            var dropToken = new DropStepState
            {
                DestinationTile = token.DestinationTile,
                HandPiece = token.HandPiece,
                Side = pieceSide
            };

            dropSequence.Next(this, ref dropToken, (int)StepAB.B);
        }
        #endregion
    }
}
