using UnityEngine;

public class PlayCardAction : IStagedAction
{
    public IPlayableLogic Playable { get; private set; }
    public CardData CardData { get; private set; }
    public PF.ActionType ActionType { get; private set; }
    public bool isCombat = false;
    private readonly int? powerIndex = null;
    private readonly string label = null;

    public int? PowerIndex => powerIndex;

    public PlayCardAction(IPlayableLogic playable, CardData cardData, PF.ActionType actionType, string label = null, int? powerIndex = null, bool isCombat = false)
    {
        this.Playable = playable;
        this.CardData = cardData;
        this.ActionType = actionType;
        this.label = label;
        this.powerIndex = powerIndex;
        this.isCombat = isCombat;
    }

    public string GetLabel()
    {
        return $"{(label is null ? ActionType.ToString() : label)} {CardData.cardName}";
    }

    public void Commit()
    {
        Game.CheckContext.Traits.AddRange(CardData.traits);
        Playable.Execute(powerIndex);
    }
}
