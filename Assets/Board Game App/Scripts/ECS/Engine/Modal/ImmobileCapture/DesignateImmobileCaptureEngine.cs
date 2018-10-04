using Data.Enums.Player;
using Data.Step.Modal;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using Service.Modal;
using Service.Piece.Find;
using Service.Piece.ImmobileCapture;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Modal.ImmobileCapture
{
    class DesignateImmobileCaptureEngine : IStep<ImmobileCaptureStepState>, IQueryingEntitiesEngine
    {
        private ImmobileCaptureService immobileCaptureService = new ImmobileCaptureService();
        private TowerModalService towerModalService = new TowerModalService();
        private PieceFindService pieceFindService = new PieceFindService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ImmobileCaptureStepState token, int condition)
        {
            TowerModalEV modal = towerModalService.FindModalEV(entitiesDB);
            SetModalOptions(modal);
            UpdateModalView(modal);
        }

        private void SetModalOptions(TowerModalEV modal)
        {
            PieceEV pieceTier1 = pieceFindService.FindPieceEVById(modal.Tier1.ReferencedPieceId, entitiesDB).Value;
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(pieceTier1.Location.Location, entitiesDB);

            PieceEV topPiece = pieceFindService.FindTopPieceByLocation(pieceTier1.Location.Location, entitiesDB).Value;
            PlayerColor colorToEnable = topPiece.PlayerOwner.PlayerColor;

            PieceEV pieceTier2 = piecesAtLocation[1];
            PieceEV? pieceTier3 = piecesAtLocation.Count > 2 ? (PieceEV?)piecesAtLocation[2] : null;

            // TODO I think this logic is unnecessary, but I'm too scared to remove it now (all handled in TowerModalEngine).  Later refactor.
            bool noTier1CheckViolationsExist = immobileCaptureService.NoTier1CheckViolationsExist(piecesAtLocation, entitiesDB);
            bool noTier3BetrayalTwoFileMoveViolationsExist = immobileCaptureService.NoTier3BetrayalTwoFileMoveViolationsExist(piecesAtLocation, entitiesDB);

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref TowerModalEV modalToChange) =>
                {
                    modalToChange.ImmobileCaptureState.ImmobileCaptureDesignated = true;

                    modalToChange.Tier1.Enabled = noTier1CheckViolationsExist
                        && pieceTier1.PlayerOwner.PlayerColor == colorToEnable
                        && pieceTier1.PlayerOwner.PlayerColor != pieceTier2.PlayerOwner.PlayerColor;

                    modalToChange.Tier2.Enabled = pieceTier2.PlayerOwner.PlayerColor == colorToEnable
                        && (pieceTier2.PlayerOwner.PlayerColor != pieceTier1.PlayerOwner.PlayerColor
                        || (pieceTier3.HasValue && pieceTier2.PlayerOwner.PlayerColor != pieceTier3.Value.PlayerOwner.PlayerColor));

                    if (pieceTier3.HasValue)
                    {
                        modalToChange.Tier3.Enabled = noTier3BetrayalTwoFileMoveViolationsExist
                            && pieceTier3.Value.PlayerOwner.PlayerColor == colorToEnable
                            && pieceTier3.Value.PlayerOwner.PlayerColor != pieceTier2.PlayerOwner.PlayerColor;
                    }
                    else
                    {
                        modalToChange.Tier3.Enabled = false;
                    }
                });
        }

        private void UpdateModalView(TowerModalEV modal)
        {
            modal.Visibility.IsVisible.value = true;
        }
    }
}
