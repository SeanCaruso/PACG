using PACG.Data;

namespace PACG.Gameplay
{
    public abstract class CharacterLogicBase : ILogicBase
    {
        // Override these as needed
        public virtual CharacterPower? GetStartOfTurnPower(PlayerCharacter pc) => null;

        public virtual CharacterPower? GetEndOfTurnPower(PlayerCharacter pc) => null;
        // etc.
    }
}
