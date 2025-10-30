using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ������ ��Ż�� ����� ��, �÷��̾��� ���� ��ġ�� �����ϰ�,
/// ī�޶��� ��ġ ������ ���� �Ŵ��� Ŭ����
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
                Debug.LogWarning($"�� '{scene.name}'�� �ߺ��� SpawnPointID�� �����մϴ�: {point.PointID}");
            }
        }

        StartCoroutine(WarpPlayerAndCamera(NextSpawnPointID));
        NextSpawnPointID = SpawnPointID.Default;
    }

    /// <summary>
    /// ������ ID�� ���� �������� �÷��̾ �̵���Ű�� �Լ�
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
                Debug.LogWarning($"��û�� ���� ���� '{pointID}'�� ã�� �� ���� Default ������ �����մϴ�.");
            }
            else
            {
                Debug.LogError($"���� '{pointID}' ���� ������, Default ���� ������ �����ϴ�.");
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
            Debug.Log("ī�޶� �̸�: " + vcam.name);
            Debug.Log("ī�޶��� ��ġ: " + vcam.gameObject.transform.position);
            vcam.OnTargetObjectWarped(warpedTarget, positionDelta);

            Debug.Log("���� �� ī�޶��� ��ġ: " + vcam.gameObject.transform.position);
        }
    }

    private IEnumerator WarpPlayerAndCamera(SpawnPointID pointID)
    {
        // �� �ε� �� ��� ���� ����ȭ�� ������ �� ������ ���
        yield return null;

        PlayerContext player = FindObjectOfType<PlayerContext>();
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (player == null || player.LookPoint == null || brain == null)
        {
            Debug.LogError("Player, LookPoint, �Ǵ� CinemachineBrain�� ã�� �� �����ϴ�!");
            yield break;
        }

        // 1���� Ȱ��ȭ�� ���� ī�޶� ã��, ���� Follow Ÿ��(LookPoint)�� ���
        var activeVirtualCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (activeVirtualCamera == null)
        {
            Debug.LogError("Ȱ��ȭ�� CinemachineVirtualCamera�� ã�� �� �����ϴ�!");
            yield break;
        }
        Transform originalFollowTarget = activeVirtualCamera.Follow;

        // �����̵� ������ Follow Ÿ���� �÷��̾� ��ü�� ��� ����
        activeVirtualCamera.Follow = player.transform;

        // --- �÷��̾� �̵� ���� ---
        Vector3 previousPosition = player.transform.position; // ������ player.transform

        SpawnPoint targetSpawnPoint;
        if (!spawnPointsInScene.TryGetValue(pointID, out targetSpawnPoint))
            spawnPointsInScene.TryGetValue(SpawnPointID.Default, out targetSpawnPoint);

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.transform.position = PositioningUtility.GetGroundedPosition(targetSpawnPoint.transform.position, player.GetComponent<Collider>());
        player.transform.rotation = targetSpawnPoint.transform.rotation;

        if (controller != null) controller.enabled = true;
        // --- �÷��̾� �̵� ���� �� ---

        // �÷��̾� ��ü�� �������� �̵����� ����ϰ� ī�޶� �����̵�
        Vector3 positionDelta = player.transform.position - previousPosition;
        activeVirtualCamera.OnTargetObjectWarped(player.transform, positionDelta);

        // ��� ���� ���� ��, Follow Ÿ���� �������(LookPoint) ����
        activeVirtualCamera.Follow = originalFollowTarget;
    }
}
