using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject mainCameraPrefab;
    [SerializeField] private GameObject lockOnCameraPrefab;
    [SerializeField] private GameObject playerCameraPrefab;

    [SerializeField] private GameObject playerInstance;
    public GameObject PlayerInstance
    {
        get { return playerInstance; }
        private set { playerInstance = value; }
    }

    private GameObject mainCameraInstance;
    public GameObject MainCameraInstance
    {
        get { return mainCameraInstance; }
        private set { mainCameraInstance = value; }
    }

    private GameObject lockOnCameraInstance;
    public GameObject LockOnCameraInstance
    {
        get { return lockOnCameraInstance; }
        private set { lockOnCameraInstance = value; }
    }

    private GameObject playerCameraInstance;
    public GameObject PlayerCameraInstance
    {
        get { return playerCameraInstance; }
        private set { playerCameraInstance = value; }
    }

    public void Initialize()
    {
        PlayerInstance = Instantiate(playerPrefab);
        DontDestroyOnLoad(PlayerInstance);       
        PlayerInstance.transform.SetParent(null); 

        MainCameraInstance = Instantiate(mainCameraPrefab, transform);
        LockOnCameraInstance = Instantiate(lockOnCameraPrefab, transform);
        PlayerCameraInstance = Instantiate(playerCameraPrefab, transform);
    }

    public void ActiveGame()
    {
        playerInstance.SetActive(true);
        playerCameraInstance.SetActive(true);
        mainCameraInstance.SetActive(true);
    }

    public void DeActiveGame()
    {
        playerInstance.SetActive(false);
        playerCameraInstance.SetActive(false);
        mainCameraInstance.SetActive(false);
    }
}
