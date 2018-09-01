using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Back.DragonKing
{
    class DragonKingAbilities : IAbilities
    {
        public List<PreMoveAbility> PreMove
        {
            get
            {
                return new List<PreMoveAbility>(new PreMoveAbility[] { PreMoveAbility.CANNOT_MOBILE_RANGE_EXPANSION });
            }
        }

        public List<DropAbility> Drop
        {
            get
            {
                return new List<DropAbility>();
            }
        }

        public List<PostMoveAbility> PostMove
        {
            get
            {
                return new List<PostMoveAbility>();
            }
        }
    }
}
