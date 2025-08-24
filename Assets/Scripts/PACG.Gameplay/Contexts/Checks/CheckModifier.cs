using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class CheckModifier
    {
        public CardInstance SourceCard { get; }

        // Skill modifications
        public List<PF.Skill> RestrictedSkills { get; set; } = new();
        public List<PF.Skill> AddedValidSkills { get; set; } = new();
        public List<string> RequiredTraits { get; set; } = new();
        public HashSet<string> ProhibitedTraits { get; set; } = new();
        
        // Category modifications
        public CheckCategory? RestrictedCategory { get; set; }
        
        // Dice modifications
        public List<int> AddedDice { get; set; } = new();
        public int AddedBonus { get; set; }
        public int SkillDiceToAdd { get; set; }
        public int? DieOverride { get; set; }
        
        // Trait modifications
        public List<string> AddedTraits { get; set; } = new();
        
        public CheckModifier(CardInstance sourceCard)
        {
            SourceCard = sourceCard;
        }
    }
}
