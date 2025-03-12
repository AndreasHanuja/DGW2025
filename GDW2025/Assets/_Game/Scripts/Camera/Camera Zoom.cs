using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = -100f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 300f;
    private float cameraZoom;
    private Vector3 cameraOffsetDirection;

    public void Start()
    {
        cameraZoom = transform.localPosition.magnitude;
        cameraOffsetDirection = transform.localPosition.normalized;
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            cameraZoom *= Mathf.Clamp(1f + (scrollInput * zoomSpeed) , 0f , 2f);
            cameraZoom = Mathf.Clamp(cameraZoom, minZoom, maxZoom);
            transform.localPosition = cameraOffsetDirection * cameraZoom;
        }
    }
}

