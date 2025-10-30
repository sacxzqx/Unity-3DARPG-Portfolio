/// <summary>
/// 씬 전환 시 상태를 초기화해야 하는 객체를 위한 인터페이스
/// </summary>
public interface IReset
{
    void ResetBeforeSceneLoad();

    void ResetAfterSceneLoad();
}
