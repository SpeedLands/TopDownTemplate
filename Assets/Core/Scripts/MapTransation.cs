using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D newMapBoundary;

    [SerializeField] private CinemachineConfiner2D cameraConfiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;
    [SerializeField] float additivePosition = 2f;
    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Teleport
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cameraConfiner == null || newMapBoundary == null)
        {
            Debug.LogError("Asigna el Camera Confiner y el New Map Boundary en el Inspector.");
            return;
        }

        if (collision.CompareTag("Player"))
        {
            cameraConfiner.BoundingShape2D = newMapBoundary;
            UpdatePlayerPosition(collision.gameObject);
            MapController_Manual.Instance?.HighLightArea(newMapBoundary.name);
            MapController_Dynamic.Instance?.UpdateCurrentArea(newMapBoundary.name);

            cameraConfiner.InvalidateBoundingShapeCache();
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            player.transform.position = teleportTargetPosition.position;
            return;
        }
        Vector3 newPosition = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                newPosition.y += additivePosition;
                break;
            case Direction.Down:
                newPosition.y -= additivePosition;
                break;
            case Direction.Left:
                newPosition.x += additivePosition;
                break;
            case Direction.Right:
                newPosition.x -= additivePosition;
                break;
        }

        player.transform.position = newPosition;
    }
}