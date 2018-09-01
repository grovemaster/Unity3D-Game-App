using Data.Enums.Piece;
using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using Service.Piece.Factory;
using System;
using System.Collections.Generic;

namespace Data.Piece.Map
{
    public static class AbilityToPiece
    {
        private static readonly Dictionary<PreMoveAbility, List<PieceType>> PreMove = new Dictionary<PreMoveAbility, List<PieceType>>();
        private static readonly Dictionary<DropAbility, List<PieceType>> Drop = new Dictionary<DropAbility, List<PieceType>>();
        private static readonly Dictionary<PostMoveAbility, List<PieceType>> PostMove = new Dictionary<PostMoveAbility, List<PieceType>>();

        static AbilityToPiece()
        {
            PieceFactory pieceFactory = new PieceFactory();
            Array pieceTypes = Enum.GetValues(typeof(PieceType));

            foreach (PieceType pieceType in pieceTypes)
            {
                IPieceData pieceData = pieceFactory.CreateIPieceData(pieceType);
                AddPreMoveAbilities(pieceData);
                AddPostMoveAbilities(pieceData);
                AddDropAbilities(pieceData);
            }
        }

        public static bool HasAbility(PreMoveAbility ability, PieceType pieceType)
        {
            return PreMove[ability].Contains(pieceType);
        }

        public static bool HasAbility(DropAbility ability, PieceType pieceType)
        {
            return Drop[ability].Contains(pieceType);
        }

        public static bool HasAbility(PostMoveAbility ability, PieceType pieceType)
        {
            return PostMove[ability].Contains(pieceType);
        }

        private static void AddPreMoveAbilities(IPieceData pieceData)
        {
            foreach (PreMoveAbility preMoveAbility in pieceData.Abilities.PreMove)
            {
                if (!PreMove.ContainsKey(preMoveAbility))
                {
                    PreMove[preMoveAbility] = new List<PieceType>();
                }

                PreMove[preMoveAbility].Add(pieceData.TypeOfPiece);
            }
        }

        private static void AddDropAbilities(IPieceData pieceData)
        {
            foreach (DropAbility dropAbility in pieceData.Abilities.Drop)
            {
                if (!Drop.ContainsKey(dropAbility))
                {
                    Drop[dropAbility] = new List<PieceType>();
                }

                Drop[dropAbility].Add(pieceData.TypeOfPiece);
            }
        }

        private static void AddPostMoveAbilities(IPieceData pieceData)
        {
            foreach (PostMoveAbility postMoveAbility in pieceData.Abilities.PostMove)
            {
                if (!PostMove.ContainsKey(postMoveAbility))
                {
                    PostMove[postMoveAbility] = new List<PieceType>();
                }

                PostMove[postMoveAbility].Add(pieceData.TypeOfPiece);
            }
        }
    }
}
