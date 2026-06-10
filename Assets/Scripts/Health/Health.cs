using UnityEngine;

public class Health : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private bool dead;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(float _damage)
    {
        if (dead) return;

        dead = true;

        if (playerMovement != null)
        {
            playerMovement.Die();
            DinoGameManager.instance.GameOver();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
