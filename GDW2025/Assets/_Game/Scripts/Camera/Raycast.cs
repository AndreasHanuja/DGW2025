using UnityEngine;
using UnityEngine.Events;

public class Raycast : MonoBehaviour
{
    [SerializeField] private GameObject marker;
    [SerializeField] private int gridTileSize = 1;
    [SerializeField] public UnityEvent<Vector2Int> click;
    [SerializeField] public int gridSize = 32;

    // Update is called once per frame
    private void Update()
    {
        marker.transform.position = new Vector3(GetGridPosition().x, 0f , GetGridPosition().y);
        GridClick();
    }

    private Vector2Int GetGridPosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int colliderLayer = LayerMask.GetMask("Collision");

        if (Physics.Raycast(ray , out RaycastHit hit, Mathf.Infinity , colliderLayer))
        {

            Vector3 hitPosition = hit.point;
            return SnapToGrid(hitPosition , gridTileSize) + new Vector2Int((int)gridTileSize/2, (int)gridTileSize / 2);

        }

        return new Vector2Int((int) Mathf.Floor(marker.transform.position.x) , (int)Mathf.Floor(marker.transform.position.z));

    }

    private Vector2Int SnapToGrid(Vector3 position, int size)
    {
        return new Vector2Int(
            (int)Mathf.Floor(position.x / size) * size,
            (int)Mathf.Floor(position.z / size) * size
        );
    }

    private void GridClick()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector2Int pos = GetGridPosition() - new Vector2Int((int)gridTileSize / 2, (int)gridTileSize / 2);

            if (0 <= Mathf.Floor(pos.x) && Mathf.Floor(pos.x) <= gridSize && 0 <= Mathf.Floor(pos.y) && Mathf.Floor(pos.y) <= gridSize)
            {
                click.Invoke(pos);
            }

        }
    }

}
