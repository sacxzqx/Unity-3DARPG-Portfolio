using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��ų �����Կ� �巡�׵� ��ų�� �Ҵ��ϰų� ������ �� �ִ� Ŭ����
/// </summary>
public class SkillQuickSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private SkillManager skillManager;
    [field:SerializeField] public string key {  get; private set; }

    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite slotDefaultImage;
    private SkillSO assignedSkill;

    public void OnDrop(PointerEventData eventData)
    {
        SkillButton draggedSkill = eventData.pointerDrag.GetComponent<SkillButton>();

        if (!skillManager.HasLearnedSkill(draggedSkill.Skill))
        {
            return;
        }

        if (draggedSkill != null)
        {
            AssignSkill(draggedSkill.Skill);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }

    public void AssignSkill(SkillSO skill)
    {
        if (skill == null)
        {
            ClearSlot();
            return;
        }

        assignedSkill = skill;
        slotImage.sprite = skill.Icon;
        skillManager.AssignSkillToKey(skill, key);
        GameEventsManager.Instance.UIEvents.SkillAssigned(key, skill);
    }

    private void ClearSlot()
    {
        if (assignedSkill == null) return;

        skillManager.UnassignSkillFromKey(key);
        assignedSkill = null;

        slotImage.sprite = slotDefaultImage;

        GameEventsManager.Instance.UIEvents.SkillAssigned(key, null);
    }
}
