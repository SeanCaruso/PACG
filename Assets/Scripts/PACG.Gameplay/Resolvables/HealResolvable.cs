
using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class HealResolvable /*: IResolvable*/
    {
        public PlayerCharacter Player { get; private set; }
        public int Count { get; private set; }
        public List<PF.CardType> TypeFilter { get; private set; }

        public HealResolvable(PlayerCharacter player, int count, List<PF.CardType> typeFilter)
        {
            Player = player;
            Count = count;
            TypeFilter = typeFilter;
        }
    }
}
