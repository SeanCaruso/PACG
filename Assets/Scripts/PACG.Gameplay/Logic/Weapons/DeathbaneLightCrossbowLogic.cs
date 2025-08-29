

/*
[PlayableLogicFor("DeathbaneLightCrossbow")]
public class DeathbaneLightCrossbowLogic : CardLogicBase
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;
    private const int ReloadIndex = 2;

    protected override List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (!Contexts.CheckContext.StagedCardTypes.Contains(CardData.cardType))
        {
            if (Contexts.CheckContext.CheckPC == CardData.Owner
                && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
                && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards)
            {
                actions.Add(new PlayCardAction(this, CardData, ActionType.Reveal, powerIndex: RevealIndex, isCombatCheck: true));

                // TODO - Add Distant combat check logic. (And update this entire implementation to match standards.)
            }
        }
        return actions;
    }

    public override void OnStage(int? powerIndex = null)
    {
        Game.Stage(CardData);
    }

    public override void OnUndo(int? powerIndex = null)
    {
        Game.Undo(CardData);
    }

    public override void Execute(int? powerIndex = null)
    {
        if (powerIndex == RevealIndex)
        {
            // Reveal to use Dexterity or Ranged + 1d8+1.
            (Skill skill, int die, int bonus) = CardData.Owner.GetBestSkill(Skill.Dexterity, Skill.Ranged);
            Contexts.CheckContext.UsedSkill = skill;
            Contexts.CheckContext.DicePool.AddDice(1, die, bonus);
            Contexts.CheckContext.DicePool.AddDice(1, 8, 1);

            // Against an Undead bane, add another 1d8.
            if (Contexts.EncounterContext.EncounteredCardData.traits?.Contains("Undead") ?? false)
            {
                Contexts.CheckContext.DicePool.AddDice(1, 8);
            }

        }
        else if ( powerIndex == DiscardIndex || powerIndex == ReloadIndex )
        {
            throw new NotImplementedException();
        }
    }
}
*/
