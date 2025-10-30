/// <summary>
/// 적의 현재 상태를 나타내는 열거형
/// 상태에 따라 AI 행동, 애니메이션, UI 처리 방식 등이 달라짐
/// </summary>
public enum EnemyState
{
    Idle,
    Battle,
    Die
}