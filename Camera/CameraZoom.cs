using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Distance Settings")]
    [SerializeField] [Range(0f, 10f)] private float defaultDistance = 6f;
    [SerializeField] [Range(0f, 10f)] private float minimumDistance = 1f;
    [SerializeField] [Range(0f, 10f)] private float maximumDistance = 6f;

    [Header("Zoom Control Settings")]
    [SerializeField][Range(0f, 10f)] private float smoothing = 6f;
    [SerializeField][Range(0f, 10f)] private float zoomSensitivity = 1f;

    private CinemachineFramingTransposer framingTransposer;
    private CinemachineInputProvider inputProvider;

    private float currentTargetDistance;

    private void Awake()
    {
        framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        currentTargetDistance = defaultDistance;
    }

    private void Update()
    {
        Zoom();
    }

    void Zoom()
    {
        float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;

        currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minimumDistance, maximumDistance);

        float currentDistance = framingTransposer.m_CameraDistance;

        if(currentDistance == currentTargetDistance)
        {
            return; // 이미 목표 거리라면 연산 불필요
        }

        // 부드러운 보간으로 카메라 거리 조절
        float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);

        framingTransposer.m_CameraDistance = lerpedZoomValue;
    }
}
