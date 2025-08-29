using PACG.Core;

namespace PACG.Gameplay
{
    public class ScourgeImmunityExploreEffect : IExploreEffect
    {
        public int NumToIgnore { get; set; }

        public ScourgeImmunityExploreEffect(int numToIgnore = 1)
        {
            NumToIgnore = numToIgnore;
        }
        
        public void ApplyTo(CheckContext context, DicePool dicePool)
        {
        }
    }
}
