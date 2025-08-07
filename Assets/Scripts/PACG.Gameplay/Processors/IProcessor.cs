using UnityEngine;

namespace PACG.Gameplay
{
    public interface IProcessor
    {
        /// <summary>
        /// Executes a single, self-contained piece of game logic.
        /// </summary>
        void Execute();
    }
}
