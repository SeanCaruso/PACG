using PACG.Gameplay;
using System;
using UnityEngine;

namespace PACG.SharedAPI
{
    public static class DialogEvents
    {
        public static event Action<ExamineResolvable> ExamineEvent;
        public static void RaiseExamineEvent(ExamineResolvable resolvable) => ExamineEvent?.Invoke(resolvable);
    }
}
