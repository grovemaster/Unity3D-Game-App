﻿using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Default
{
    public class NoAbility : IAbilities
    {
        public List<PreMoveAbility> PreMove
        {
            get
            {
                return new List<PreMoveAbility>();
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
