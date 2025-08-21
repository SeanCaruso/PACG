using System.Collections.Generic;
using PACG.Core;
using PACG.Gameplay;
using PACG.SharedAPI;
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

        private CheckContext _checkContext;
        private readonly Dictionary<PF.Skill, SkillRow> _skillRows = new();
        private Color32 _cardTypeColor;
        private bool _isCollapsed = true;
        private float _skillRowsHeight;

        private void OnEnable()
        {
            OverlayButton.onClick.AddListener(OnShowHide);

            DialogEvents.ValidSkillsChanged += OnValidSkillsChanged;
        }

        private void OnDisable()
        {
            OverlayButton.onClick.RemoveListener(OnShowHide);

            DialogEvents.ValidSkillsChanged -= OnValidSkillsChanged;
        }

        public void SetCheckContext(CheckContext context, CardInstance card)
        {
            _checkContext = context;
            _cardTypeColor = GuiUtils.GetPanelColor(card.Data.cardType);
            
            OnValidSkillsChanged(context.GetCurrentValidSkills());

            LeftArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            RightArrow.transform.localRotation = Quaternion.Euler(0f, 0f, -180f);
        }

        private void OnValidSkillsChanged(List<PF.Skill> validSkills)
        {
            _skillRowsHeight = 0;
            _skillRows.Clear();
            
            for (var i = SkillRowsContainer.childCount - 1; i >= 0; i--)
                Destroy(SkillRowsContainer.GetChild(i).gameObject);
            
            var bestSkill = _checkContext.Character.GetBestSkill(validSkills.ToArray());
            _checkContext.UsedSkill = bestSkill.skill;

            foreach (var skill in validSkills)
            {
                var skillRow = Instantiate(SkillRowPrefab, SkillRowsContainer);

                var (die, bonus) = _checkContext.Character.GetSkill(skill);
                skillRow.DieImage.sprite = skillRow.GetDieSprite(die);
                skillRow.DieText.text = $"d{die}";
                skillRow.SkillBonusText.text = $"+ {bonus}";
                skillRow.SkillNameText.text = skill.ToString();
                skillRow.DcText.text = _checkContext.Resolvable.Difficulty.ToString();

                skillRow.BackgroundPanel.color = Color.Lerp(
                    _cardTypeColor,
                    skill == bestSkill.skill ? Color.gray : Color.black,
                    0.75f);

                skillRow.OverlayButton.onClick.AddListener(() => OnRowClicked(skill));

                _skillRows[skill] = skillRow;

                _skillRowsHeight += skillRow.GetComponent<RectTransform>().rect.height;
            }

            SkillNameText.text = bestSkill.skill.ToString().ToUpper();

            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, _isCollapsed ? _skillRowsHeight : 0f);
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

        private void OnRowClicked(PF.Skill newSkill)
        {
            _checkContext.UsedSkill = newSkill;
            SkillNameText.text = newSkill.ToString().ToUpper();
            
            foreach (var (skill, row) in _skillRows)
            {
                row.BackgroundPanel.color = Color.Lerp(
                    _cardTypeColor,
                    skill == newSkill ? Color.gray : Color.black,
                    0.75f);
            }
        }
    }
}
