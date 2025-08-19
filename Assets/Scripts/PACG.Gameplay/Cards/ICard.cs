
using System.Collections.Generic;

namespace PACG.Gameplay
{
    /// <summary>
    /// Interface for all card instances.
    /// </summary>
    public interface ICard
    {
        public string Name { get; }
        public List<string> Traits { get; }
    }
}
