using System.Collections.Generic;

namespace PACG.Gameplay
{
    /// <summary>
    /// Interface for resolvables involving checks (Combat and Skill)
    /// </summary>
    public interface ICheckResolvable
    {
        public PlayerCharacter Character { get; }
        public IReadOnlyList<PF.Skill> Skills { get; }
        public int Difficulty { get; }
    }
}
