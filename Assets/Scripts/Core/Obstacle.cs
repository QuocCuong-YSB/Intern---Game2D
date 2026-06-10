using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Ground, Air, Destructible }

    [Header("Type")]
    public ObstacleType type = ObstacleType.Ground;

    [Header("Health")]
    public int hp = 1;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Animator anim;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private bool destroyed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
        }
    }

    private void FixedUpdate()
    {
        if (destroyed || !DinoGameManager.instance.isPlaying) return;

        float speed = DinoGameManager.instance.CurrentSpeed;
        if (rb != null)
            rb.linearVelocity = new Vector2(-speed, 0);
        else
            transform.Translate(Vector2.left * speed * Time.fixedDeltaTime);

        if (transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (destroyed) return;
        hp -= damage;
        if (hp <= 0)
        {
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        destroyed = true;
        boxCollider.enabled = false;
        DinoGameManager.instance.AddScore(50);
        if (anim != null)
        {
            anim.SetTrigger("explode");
            float destroyDelay = anim.GetCurrentAnimatorStateInfo(0).length;
            Invoke(nameof(Deactivate), 0.3f);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destroyed) return;
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);
        }
    }

    private void OnDisable()
    {
        destroyed = false;
        hp = 1;
        if (boxCollider != null) boxCollider.enabled = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}
