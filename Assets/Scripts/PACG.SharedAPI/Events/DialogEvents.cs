using System;
using System.Collections.Generic;
using PACG.Core;
using PACG.Gameplay;

namespace PACG.SharedAPI
{
    public static class DialogEvents
    {
        // Turn phase events
        public static event Action<PlayerCharacter, GameServices> MoveClickedEvent;
        public static void RaiseMoveClickedEvent(PlayerCharacter pc, GameServices gameServices) =>
            MoveClickedEvent?.Invoke(pc, gameServices);
        
        // Deck examine events
        public static event Action<ExamineContext> ExamineEvent;
        public static void RaiseExamineEvent(ExamineContext examineContext) => ExamineEvent?.Invoke(examineContext);
        
        // Skill selection events
        public static event Action<CheckContext> CheckStartEvent;
        public static void RaiseCheckStartEvent(CheckContext checkContext) => CheckStartEvent?.Invoke(checkContext);

        public static event Action<List<Skill>> ValidSkillsChanged;
        public static void RaiseValidSkillsChanged(List<Skill> validSkills) => ValidSkillsChanged?.Invoke(validSkills);
        
        public static event Action CheckEndEvent;
        public static void RaiseCheckEndEvent() => CheckEndEvent?.Invoke();
    }
}
