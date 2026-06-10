using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [Header("Scrolling")]
    [SerializeField] private float tileWidth = 2.39f;
    [SerializeField] private float despawnX = -10f;

    [Header("References")]
    [SerializeField] private Transform[] groundTiles;
    [SerializeField] private Transform[] backgroundLayers;

    private float totalWidth;

    private void Awake()
    {
        totalWidth = tileWidth * groundTiles.Length;
    }

    private void FixedUpdate()
    {
        if (DinoGameManager.instance == null || !DinoGameManager.instance.isPlaying) return;

        float speed = DinoGameManager.instance.CurrentSpeed;

        foreach (Transform tile in groundTiles)
        {
            tile.Translate(Vector2.left * speed * Time.fixedDeltaTime);
            if (tile.position.x < despawnX)
            {
                tile.position = new Vector3(
                    tile.position.x + totalWidth,
                    tile.position.y,
                    tile.position.z
                );
            }
        }

        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            float bgSpeed = speed * (0.1f + i * 0.2f);
            backgroundLayers[i].Translate(Vector2.left * bgSpeed * Time.fixedDeltaTime);

            if (backgroundLayers[i].position.x < despawnX * 3)
            {
                backgroundLayers[i].position = new Vector3(
                    tileWidth * 3,
                    backgroundLayers[i].position.y,
                    backgroundLayers[i].position.z
                );
            }
        }
    }
}
