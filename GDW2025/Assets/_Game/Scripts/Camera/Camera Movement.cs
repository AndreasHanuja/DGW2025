using UnityEngine;
using UnityEngine.Events;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 100f;
    [SerializeField] Vector2 minCameraMovePoint = new(0, 0);
    [SerializeField] Vector2 maxCameraMovePoint = new(32, 32);

    // Der Input des Spielers wird FPS-unabhängig und mit dem Speed multipliziert als Movement ausgegeben.
    public void Update()
    {
        float xValue = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float zValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        float rotation = ((Input.GetKey(KeyCode.Q) ? -1f : 0f) + (Input.GetKey(KeyCode.E) ? 1f : 0f)) * Time.deltaTime * moveSpeed;

        transform.Translate(xValue, 0, zValue, Space.Self);
        transform.localPosition = new Vector3 (Mathf.Clamp(transform.localPosition.x, minCameraMovePoint.x, maxCameraMovePoint.x), 0f, Mathf.Clamp(transform.localPosition.z, minCameraMovePoint.y, maxCameraMovePoint.y));
        transform.Rotate(Vector3.up, rotation);
    }


}
