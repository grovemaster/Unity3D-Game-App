using Svelto.ECS;

namespace ECS.Component.Modal
{
    /**
     * This interface is never an EV member
     * 
     * Can Svelto ECS's Implementer differentiate between multiple components of the same type?
     * Example: IWeapon MainHand, IWeapon OffHand
     * 
     * I have my doubts, so to avoid those potential complications, while not duplicating code,
     * I've chosen this design
     */
    public interface ITowerOptionBase : IComponent
    {
        string Name { get; set; }
        bool Enabled { get; set; }
        int ReferencedPieceId { get; set; } // Piece of tower tier, clicked value is set as Answer
    }

    public interface ITier1OptionComponent: IComponent, ITowerOptionBase
    { }

    public interface ITier2OptionComponent : IComponent, ITowerOptionBase
    { }

    public interface ITier3OptionComponent : IComponent, ITowerOptionBase
    { }
}
