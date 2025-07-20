using UnityEngine;

[EncounterLogicFor("Mercenary")]
public class MercenaryLogic : IEncounterLogic
{
    public void Execute(EncounterContext context, EncounterPhase phase)
    {
        if (phase == EncounterPhase.Resolve)
        {
            IResolvable resolvable;
            if (!context.CheckResult.WasSuccess)
            {
                resolvable = new CardActionResolvable(context.ActivePlayer, PF.ActionType.Bury, 1, new() { PF.CardType.Weapon, PF.CardType.Armor });
            }
            else
            {
                resolvable = new HealResolvable(context.ActivePlayer, 1, new() { PF.CardType.Weapon, PF.CardType.Armor });
            }
        }
    }
}
