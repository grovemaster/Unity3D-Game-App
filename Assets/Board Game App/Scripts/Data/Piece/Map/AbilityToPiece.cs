using Data.Enums.Piece;
using Data.Enums.Piece.PostMove;
using ECS.EntityView.Piece;
using System;
using System.Collections.Generic;

namespace Data.Piece.Map
{
    public static class AbilityToPiece
    {
        private static readonly Dictionary<PostMoveAbility, PieceEV> PostMove;

        static AbilityToPiece()
        {
            //IEnumerable<PieceType> pieceTypes = Enum.GetValues
        }
        /**
         * private static readonly Map<PreMove, PieceType> preMove
  Static get property for PreMove
 Same for other abilities

  static AbilityToPiece()
    IEnumerable<PieceType> pieceTypes = PieceType.GetValues(typeof(PieceType)).Cast<PieceType>();

    For loop pieceTypes
      Create IPieceData
      Assign PreMove map if there's a value, same with other maps
         */
    }
}
