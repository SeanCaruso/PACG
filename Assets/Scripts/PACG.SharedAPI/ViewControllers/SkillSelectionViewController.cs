using PACG.Gameplay;
using PACG.Presentation.SkillSelectionDialog;
using UnityEngine;

namespace PACG.SharedAPI
{
    public class SkillSelectionViewController : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform SkillSelectionContainer;
        
        [Header("Prefabs")]
        public SkillSelectionDialog DialogPrefab;

        private void OnEnable()
        {
            DialogEvents.CheckStartEvent += OnCheckStart;
            DialogEvents.CheckEndEvent += OnCheckEnd;
        }
        
        private void OnDisable()
        {
            DialogEvents.CheckStartEvent -= OnCheckStart;
            DialogEvents.CheckEndEvent -= OnCheckEnd;
        }

        private void OnCheckStart(CheckContext context)
        {
            var dialog = Instantiate(DialogPrefab, SkillSelectionContainer, false);
            dialog.SetCheckContext(context);
        }

        private void OnCheckEnd()
        {
            for (var i = SkillSelectionContainer.childCount - 1; i >= 0; i--)
                Destroy(SkillSelectionContainer.GetChild(i).gameObject);
        }
    }
}
