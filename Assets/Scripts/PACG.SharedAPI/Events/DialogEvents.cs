using PACG.Gameplay;
using System;

namespace PACG.SharedAPI
{
    public static class DialogEvents
    {
        // Deck examine events
        public static event Action<ExamineContext> ExamineEvent;
        public static void RaiseExamineEvent(ExamineContext examineContext) => ExamineEvent?.Invoke(examineContext);
        
        // Skill selection events
        public static event Action<CheckContext> CheckStartEvent;
        public static void RaiseCheckStartEvent(CheckContext checkContext) => CheckStartEvent?.Invoke(checkContext);
        
        public static event Action CheckEndEvent;
        public static void RaiseCheckEndEvent() => CheckEndEvent?.Invoke();
    }
}
