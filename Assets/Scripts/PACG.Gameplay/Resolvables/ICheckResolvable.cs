using System.Collections.Generic;

namespace PACG.Gameplay
{
    /// <summary>
    /// Interface for resolvables involving checks (Combat and Skill)
    /// </summary>
    public interface ICheckResolvable
    {
        public LogicRegistry LogicRegistry { get; }
        public PlayerCharacter Character { get; }
        public int Difficulty { get; }
    }
}