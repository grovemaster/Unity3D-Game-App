using Data.Enums.Piece;
using Data.Enums.Player;
using ECS.Component.Modal;
using ECS.Implementor;
using Svelto.ECS;
using UnityEngine;

namespace UI.Modal
{
    /**
     * This class should never be directly attached to a GameObject, but used polymorphically for the tower tiers
     */
    abstract class TowerOptionBase : MonoBehaviour, IImplementor, ITowerOptionBase
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int ReferencedPieceId { get; set; }
        public DispatchOnSet<int> Answer { get; set; }
        public PlayerColor Team { get; set; }
        public PieceType PieceType { get; set; }
        public PieceType Back { get; set; }

        void Awake()
        {
            Answer = new DispatchOnSet<int>(gameObject.GetInstanceID());
        }
    }
}
