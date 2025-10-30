/// <summary>
/// 스킬 사용 시 반환되는 결과 상태를 정의하는 열거형
/// </summary>
public enum SkillUseResult
{
    Success,
    NotAssigned,
    NotEnoughMana,
    OnCooldown
}