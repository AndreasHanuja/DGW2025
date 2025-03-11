using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = -1f;
    [SerializeField] private float maxZoom = -20f;

    private CinemachineFollow cameraZoom;

    void Start()
    {
        cameraZoom = GetComponent<CinemachineFollow>();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Wenn das Mausrad bewegt wird, ändern wir den Field of View (FOV)
        if (scrollInput != 0)
        {
            cameraZoom.FollowOffset.z -= scrollInput * zoomSpeed;
            cameraZoom.FollowOffset.z = Mathf.Clamp(cameraZoom.FollowOffset.z, maxZoom, minZoom);
        }
    }
}

