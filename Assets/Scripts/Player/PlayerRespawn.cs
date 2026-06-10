using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Speed Boost")]
    [SerializeField] private float speedBoostAmount = 1.2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip checkpointSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>()?.SetTrigger("activate");

            if (DinoGameManager.instance != null)
            {
                DinoGameManager.instance.IncreaseSpeed();
            }
        }
    }
}