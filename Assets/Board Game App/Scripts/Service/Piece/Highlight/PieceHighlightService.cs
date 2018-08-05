using Data.Enum.Player;
using ECS.EntityView.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;

namespace Service.Piece.Highlight
{
    class PieceHighlightService
    {
        public List<PieceEV> DeHighlightPlayerPieces(PlayerColor pieceTeam, IEntitiesDB entitiesDB)
        {
            List<PieceEV> pieces = PieceService.FindPiecesByTeam(pieceTeam, entitiesDB)
                .Where(piece => piece.Highlight.IsHighlighted).ToList();

            DeHighlightPlayerPieces(pieces, entitiesDB);

            return pieces;
        }

        public List<PieceEV> DeHighlightOtherPlayerPieces(
            int pieceToNotChangeEntityId, PlayerColor pieceTeam, IEntitiesDB entitiesDB)
        {
            List<PieceEV> pieces = PieceService.FindPiecesByTeam(pieceTeam, entitiesDB)
                .Where(piece => piece.ID.entityID != pieceToNotChangeEntityId && piece.Highlight.IsHighlighted).ToList();

            DeHighlightPlayerPieces(pieces, entitiesDB);

            return pieces;
        }

        private void DeHighlightPlayerPieces(List<PieceEV> pieces, IEntitiesDB entitiesDB)
        {
            foreach (PieceEV piece in pieces)
            {
                entitiesDB.ExecuteOnEntity(
                    piece.ID,
                    (ref PieceEV pieceToChange) =>
                    {
                        pieceToChange.Highlight.IsHighlighted = false;
                        pieceToChange.Highlight.CurrentColorStates.Clear();
                    });

                piece.ChangeColorTrigger.PlayChangeColor = true;
            }
        }
    }
}
