
using System.Collections.Generic;
using PACG.Data;

namespace PACG.Gameplay
{
    /// <summary>
    /// Interface for all card instances.
    /// </summary>
    public interface ICard
    {
        public string Name { get; }
        public CardType CardType { get; }
        public List<string> Traits { get; }
    }
}
