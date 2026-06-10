using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpPower;

    [Header("Duck")]
    [SerializeField] private float duckColliderHeight = 0.5f;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    [Header("Ground")]
    [SerializeField] private float groundY = -1.68f;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private bool isDucking;
    private bool isDead;

    public bool IsGrounded { get; private set; }
    public bool IsDucking => isDucking;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
        body.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    private void Update()
    {
        if (!DinoGameManager.instance.isPlaying) return;

        if (!isDead)
        {
            HandleDuck();
            HandleJump();
        }

        anim.SetBool("run", true);
        anim.SetBool("grounded", IsGrounded);
        anim.SetBool("duck", isDucking);
    }

    private void FixedUpdate()
    {
        if (!DinoGameManager.instance.isPlaying) return;

        if (transform.position.y <= groundY)
        {
            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;

            if (body.linearVelocity.y < 0)
                body.linearVelocity = new Vector2(0, 0);

            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private void HandleDuck()
    {
        bool duckPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (duckPressed && IsGrounded && !isDucking)
        {
            isDucking = true;
            boxCollider.size = new Vector2(originalColliderSize.x, duckColliderHeight);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y - duckColliderHeight) / 2f);
        }
        else if (!duckPressed && isDucking)
        {
            isDucking = false;
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && !isDucking)
        {
            SoundManager.instance.PlaySound(jumpSound);
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        }

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
    }

    public bool canAttack()
    {
        return IsGrounded && !isDucking;
    }

    public void Die()
    {
        isDead = true;
        body.linearVelocity = Vector2.zero;
    }
}
