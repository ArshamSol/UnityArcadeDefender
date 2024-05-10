using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f; // Speed of player movement
    public float verticalBoundary = 4.5f; // Vertical limit for player movement
    public float backwardScrollSpeedFactor = 0.5f; // Speed reduction factor when moving backward
    public float forwardScrollSpeedIncrease = 0.5f; // Speed increase factor when moving forward
    public SpriteRenderer shipSpriteRenderer;
    public bool facingRight = true; // Is the player facing right initially
    public float wrapDuration = 1f; // Time to wrap smoothly

    private float verticalInput;
    private Vector3 velocity;

    void Update()
    {
        HandleInput();
        MovePlayer();
        CheckDirectionChange();
    }

    private void HandleInput()
    {
        verticalInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer()
    {
        velocity.y = Mathf.Lerp(velocity.y, verticalInput * moveSpeed, Time.deltaTime);
        transform.Translate(velocity * Time.deltaTime);

        // Clamp vertical movement to screen boundaries
        float clampedY = Mathf.Clamp(transform.position.y, -verticalBoundary, verticalBoundary);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    private void CheckDirectionChange()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0 && !facingRight)
        {
            facingRight = true;
            FlipSprite();
            StartWrap();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            facingRight = false;
            FlipSprite();
            StartWrap();
        }
    }

    private void FlipSprite()
    {
        shipSpriteRenderer.flipX = !shipSpriteRenderer.flipX;
    }

    private void StartWrap()
    {
        ScrollingBackground.Instance.StartSmoothWrap(facingRight, wrapDuration);
    }
}