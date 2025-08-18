using PACG.Gameplay;
using System;
using UnityEngine;

namespace PACG.SharedAPI
{
    public static class DialogEvents
    {
        public static event Action<ExamineContext> ExamineEvent;
        public static void RaiseExamineEvent(ExamineContext examineContext) => ExamineEvent?.Invoke(examineContext);
    }
}
