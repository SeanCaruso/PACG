using System.Collections.Generic;
using UnityEngine;

public class CardActionResolvable /*: IResolvable*/
{
    public PlayerCharacter Player {  get; private set; }
    public PF.ActionType Verb {  get; private set; }
    public int Count { get; private set; }
    public List<PF.CardType> TypeFilter { get; private set; }

    public CardActionResolvable(PlayerCharacter player, PF.ActionType verb, int count, List<PF.CardType> typeFilter)
    {
        Player = player;
        Verb = verb;
        Count = count;
        TypeFilter = typeFilter;
    }

    public List<PlayCardAction> GetValidActions(ActionContext context, ResolutionManager resolutionManager)
    {
        throw new System.NotImplementedException();
    }
}
