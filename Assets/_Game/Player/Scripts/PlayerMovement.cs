using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Controllers")]

    [Header("Config")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool enableDoubleJump = false; // começa desativado, só libera no quiz

    private Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    private bool isGrounded;
    public bool isInBush;
    private bool jumpPressed;
    private int jumpCount = 0;
    private int maxJumps = 2;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isCollidingHorizontally = false;
    private float horizontalInput;

    public static PlayerMovement instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.sharedMaterial = new PhysicsMaterial2D { friction = 0 };
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (!isInBush)
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        else
            animator.SetFloat("Crawl", Mathf.Abs(horizontalInput));

        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpCount = 0;
            animator.SetTrigger("Land");
        }

        MoveWithCollisionDetection();

        if (jumpPressed)
        {
            if (isGrounded && jumpCount == 0)
            {
                Jump();
            }
            else if (enableDoubleJump && jumpCount == 1 && jumpCount < maxJumps)
            {
                Jump();
            }
            jumpPressed = false;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("Jump");
        jumpCount++;
    }

    void MoveWithCollisionDetection()
    {
        isCollidingHorizontally = CheckHorizontalCollision();
        bool canMove = !isCollidingHorizontally ||
                       (isCollidingHorizontally && IsMovingAwayFromCollision());

        if (canMove)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    bool CheckHorizontalCollision()
    {
        Vector2 direction = new Vector2(Mathf.Sign(horizontalInput), 0);
        if (horizontalInput == 0) return false;

        Vector2 center = rb.position;
        float height = GetComponent<Collider2D>().bounds.size.y;
        float width = GetComponent<Collider2D>().bounds.size.x / 2;

        Vector2[] raycastOrigins = new Vector2[3]
        {
            new Vector2(center.x, center.y + height * 0.4f),
            center,
            new Vector2(center.x, center.y - height * 0.4f)
        };

        float rayLength = width + 0.05f;

        for (int i = 0; i < raycastOrigins.Length; i++)
        {
            Debug.DrawRay(raycastOrigins[i], direction * rayLength, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigins[i], direction, rayLength, groundLayer);
            if (hit.collider != null)
                return true;
        }
        return false;
    }

    bool IsMovingAwayFromCollision()
    {
        bool collidingRight = false;
        bool collidingLeft = false;

        Collider2D collider = GetComponent<Collider2D>();
        Vector2 rightCheck = new Vector2(collider.bounds.max.x, rb.position.y);
        Vector2 leftCheck = new Vector2(collider.bounds.min.x, rb.position.y);

        Collider2D hitRight = Physics2D.OverlapPoint(rightCheck, groundLayer);
        if (hitRight != null) collidingRight = true;

        Collider2D hitLeft = Physics2D.OverlapPoint(leftCheck, groundLayer);
        if (hitLeft != null) collidingLeft = true;

        if (collidingRight && horizontalInput < 0) return true;
        if (collidingLeft && horizontalInput > 0) return true;

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}