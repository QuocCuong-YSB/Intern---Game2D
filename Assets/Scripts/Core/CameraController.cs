using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fixedX = 0f;
    [SerializeField] private float fixedY = 0f;

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(fixedX, fixedY, transform.position.z);
    }
}
