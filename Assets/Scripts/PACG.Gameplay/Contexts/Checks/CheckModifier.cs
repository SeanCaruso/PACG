using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class CheckModifier
    {
        public CardInstance SourceCard { get; }

        // Skill modifications
        public List<PF.Skill> RestrictedSkills { get; set; } = new();
        public List<PF.Skill> AddedValidSkills { get; set; } = new();
        
        // Category modifications
        public CheckCategory? RestrictedCategory { get; set; } = null;
        
        // Dice modifications
        public List<int> AddedDice { get; set; } = new();
        public int AddedBonus { get; set; }
        public int SkillDiceToAdd { get; set; }
        
        // Trait modifications
        public List<string> AddedTraits { get; set; } = new();
        
        public CheckModifier(CardInstance sourceCard)
        {
            SourceCard = sourceCard;
        }
    }
}
