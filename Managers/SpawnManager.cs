using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 내에서 포탈을 사용할 때, 플레이어의 스폰 위치를 지정하고,
/// 카메라의 위치 조절을 돕는 매니저 클래스
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public static SpawnPointID NextSpawnPointID { get; set; } = SpawnPointID.Default;

    public bool IsLoadingFromSave = false;

    private Dictionary<SpawnPointID, SpawnPoint> spawnPointsInScene = new Dictionary<SpawnPointID, SpawnPoint>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Loading")
        {
            return;
        }

        if (IsLoadingFromSave)
        {
            IsLoadingFromSave = false;
            return;
        }

        spawnPointsInScene.Clear();
        SpawnPoint[] points = FindObjectsOfType<SpawnPoint>();

        foreach (var point in points)
        {
            if (!spawnPointsInScene.ContainsKey(point.PointID))
            {
                spawnPointsInScene.Add(point.PointID, point);
            }
            else
            {
                Debug.LogWarning($"씬 '{scene.name}'에 중복된 SpawnPointID가 존재합니다: {point.PointID}");
            }
        }

        StartCoroutine(WarpPlayerAndCamera(NextSpawnPointID));
        NextSpawnPointID = SpawnPointID.Default;
    }

    /// <summary>
    /// 지정된 ID의 스폰 지점으로 플레이어를 이동시키는 함수
    /// </summary>
    public void SpawnPlayerAt(SpawnPointID pointID)
    {
        PlayerContext player = FindObjectOfType<PlayerContext>();
        if (player == null) return;

        Vector3 previousPosition = player.transform.position;
        SpawnPoint targetSpawnPoint = null;

        if (spawnPointsInScene.TryGetValue(pointID, out targetSpawnPoint))
        {
            if (spawnPointsInScene.TryGetValue(pointID, out targetSpawnPoint))
            {
                Debug.LogWarning($"요청된 스폰 지점 '{pointID}'를 찾을 수 없어 Default 지점에 스폰합니다.");
            }
            else
            {
                Debug.LogError($"씬에 '{pointID}' 스폰 지점도, Default 스폰 지점도 없습니다.");
                return;
            }
        }

        Collider playerCollider = player.GetComponent<Collider>();
        player.transform.position = PositioningUtility.GetGroundedPosition(targetSpawnPoint.transform.position, playerCollider);
        player.transform.rotation = targetSpawnPoint.transform.rotation;

        Vector3 positionDelta = player.transform.position - previousPosition;
        TeleportAllVirtualCameras(player.transform, positionDelta);
    }

    private void TeleportAllVirtualCameras(Transform warpedTarget, Vector3 positionDelta)
    {
        var allVCams = FindObjectsOfType<CinemachineVirtualCameraBase>(true);

        foreach (var vcam in allVCams)
        {
            Debug.Log("카메라 이름: " + vcam.name);
            Debug.Log("카메라의 위치: " + vcam.gameObject.transform.position);
            vcam.OnTargetObjectWarped(warpedTarget, positionDelta);

            Debug.Log("텔포 후 카메라의 위치: " + vcam.gameObject.transform.position);
        }
    }

    private IEnumerator WarpPlayerAndCamera(SpawnPointID pointID)
    {
        // 씬 로드 후 모든 것이 안정화될 때까지 한 프레임 대기
        yield return null;

        PlayerContext player = FindObjectOfType<PlayerContext>();
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (player == null || player.LookPoint == null || brain == null)
        {
            Debug.LogError("Player, LookPoint, 또는 CinemachineBrain을 찾을 수 없습니다!");
            yield break;
        }

        // 1현재 활성화된 가상 카메라를 찾고, 원래 Follow 타겟(LookPoint)을 기억
        var activeVirtualCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (activeVirtualCamera == null)
        {
            Debug.LogError("활성화된 CinemachineVirtualCamera를 찾을 수 없습니다!");
            yield break;
        }
        Transform originalFollowTarget = activeVirtualCamera.Follow;

        // 순간이동 직전에 Follow 타겟을 플레이어 본체로 잠시 변경
        activeVirtualCamera.Follow = player.transform;

        // --- 플레이어 이동 로직 ---
        Vector3 previousPosition = player.transform.position; // 기준은 player.transform

        SpawnPoint targetSpawnPoint;
        if (!spawnPointsInScene.TryGetValue(pointID, out targetSpawnPoint))
            spawnPointsInScene.TryGetValue(SpawnPointID.Default, out targetSpawnPoint);

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.transform.position = PositioningUtility.GetGroundedPosition(targetSpawnPoint.transform.position, player.GetComponent<Collider>());
        player.transform.rotation = targetSpawnPoint.transform.rotation;

        if (controller != null) controller.enabled = true;
        // --- 플레이어 이동 로직 끝 ---

        // 플레이어 본체를 기준으로 이동량을 계산하고 카메라를 순간이동
        Vector3 positionDelta = player.transform.position - previousPosition;
        activeVirtualCamera.OnTargetObjectWarped(player.transform, positionDelta);

        // 모든 것이 끝난 후, Follow 타겟을 원래대로(LookPoint) 복귀
        activeVirtualCamera.Follow = originalFollowTarget;
    }
}
