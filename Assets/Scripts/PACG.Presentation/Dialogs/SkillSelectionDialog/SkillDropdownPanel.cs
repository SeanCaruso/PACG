using PACG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation.SkillSelectionDialog
{
    public class SkillDropdownPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform SkillRowsContainer;
        public TextMeshProUGUI SkillNameText;
        public Image LeftArrow;
        public Image RightArrow;
        public Button OverlayButton;

        [Header("Prefabs")]
        public SkillRow SkillRowPrefab;

        private bool _isCollapsed;
        private float _skillRowsHeight = 0f;

        private void OnEnable()
        {
            OverlayButton.onClick.AddListener(OnShowHide);
        }

        private void OnDisable()
        {
            OverlayButton.onClick.RemoveListener(OnShowHide);
        }

        public void SetCheckContext(CheckContext context)
        {
            foreach (var skill in context.GetCurrentValidSkills())
            {
                var skillRow = Instantiate(SkillRowPrefab, SkillRowsContainer);

                var (die, bonus) = context.Character.GetSkill(skill);
                skillRow.DieImage.sprite = skillRow.GetDieSprite(die);
                skillRow.DieText.text = $"d{die}";
                skillRow.SkillBonusText.text = $"+ {bonus}";
                skillRow.SkillNameText.text = skill.ToString();
                skillRow.DcText.text = context.Resolvable.Difficulty.ToString();

                _skillRowsHeight += skillRow.GetComponent<RectTransform>().rect.height;
            }

            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, _skillRowsHeight);

            LeftArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            RightArrow.transform.localRotation = Quaternion.Euler(0f, 0f, -180f);

            _isCollapsed = true;

            SkillNameText.text = context.Character.GetBestSkill(
                context.GetCurrentValidSkills().ToArray()).skill.ToString().ToUpper();
        }

        private void OnShowHide()
        {
            const float time = 0.3f;

            var rect = GetComponent<RectTransform>();
            var targetY = _isCollapsed ? 0f : _skillRowsHeight;
            LeanTween.moveY(rect, targetY, time).setEaseOutQuart();

            LeanTween.rotateAroundLocal(
                LeftArrow.gameObject, Vector3.forward, _isCollapsed ? 180f : -180f, time).setEaseOutQuart();
            LeanTween.rotateAroundLocal(
                RightArrow.gameObject, Vector3.forward, _isCollapsed ? -180f : 180f, time).setEaseOutQuart();

            _isCollapsed = !_isCollapsed;
        }
    }
}
