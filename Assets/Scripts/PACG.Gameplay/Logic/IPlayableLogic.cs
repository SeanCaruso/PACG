using System;
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PlayableLogicForAttribute : Attribute
    {
        public string CardID { get; set; }
        public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
    }

    public interface IPlayableLogic : ICardLogic
    {
        public bool CanPlay => GetAvailableActions().Count > 0;

        public List<IStagedAction> GetAvailableActions();

        public void OnStage(IStagedAction action);

        public void OnUndo(IStagedAction action);

        public void Execute(IStagedAction action);
    }
}
