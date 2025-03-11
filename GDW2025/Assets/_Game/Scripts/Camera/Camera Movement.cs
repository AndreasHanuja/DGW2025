using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Vector2 minCameraMovePoint = new(0, 0);
    [SerializeField] Vector2 maxCameraMovePoint = new(32, 32);
    [SerializeField] GameObject collisionLayer;

    public void Start()
    {
        collisionLayer.transform.localPosition = new Vector3((maxCameraMovePoint.x + minCameraMovePoint.x) / 2, 0, (maxCameraMovePoint.y + minCameraMovePoint.y) / 2);
        collisionLayer.transform.localScale = new Vector3(maxCameraMovePoint.x / 10 , 0 , maxCameraMovePoint.y / 10);
    }

    // Der Input des Spielers wird FPS-unabhängig und mit dem Speed multipliziert als Movement ausgegeben.
    void Update()
    {
        float xValue = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float zValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        float rotationAxis = ((Input.GetKey(KeyCode.Q) ? -1f : 0f) + (Input.GetKey(KeyCode.E) ? 1f : 0f)) * Time.deltaTime * moveSpeed * 10;

        transform.Translate(xValue, 0, zValue, Space.Self);
        transform.localPosition = new Vector3 (Mathf.Clamp(transform.localPosition.x, minCameraMovePoint.x, maxCameraMovePoint.x), 0f, Mathf.Clamp(transform.localPosition.z, minCameraMovePoint.y, maxCameraMovePoint.y));
        transform.Rotate(new Vector3(0f, rotationAxis, 0f));
    }
}
