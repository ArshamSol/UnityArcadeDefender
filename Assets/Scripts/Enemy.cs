using UnityEngine;

public class Enemy : MonoBehaviour
{
    ScrollingBackground scrollingBackground = ScrollingBackground.Instance;
    private int scoreValue = 100;
    private float horizontalSpeed = 0.2f;
    private float descentSpeed = 0.2f;
    private float lowAltitude = -3f; // Define the altitude limit for switching to horizontal movement
    private bool moveRight = true; // Determines the initial horizontal movement direction
    private MovementState currentState = MovementState.Descending;
    private float wrapThreshold = 0f; // Distance from the background edges to wrap around

    public enum MovementState
    {
        Descending,
        MovingHorizontally
    }

    void Update()
    {
        CheckWrapAround();
        switch (currentState)
        {
            case MovementState.Descending:
                MoveDiagonallyDownward();
                break;
            case MovementState.MovingHorizontally:
                MoveHorizontally();
                break;
        }

    }

    private void CheckWrapAround()
    {
        if (scrollingBackground.backgroundParts.Length < 2) return;

        float leftBoundary = scrollingBackground.mainCamera.transform.position.x - scrollingBackground.spriteWidth;
        float rightBoundary = scrollingBackground.mainCamera.transform.position.x + scrollingBackground.spriteWidth;
     
        if (transform.position.x < leftBoundary - wrapThreshold)
        {
            transform.position = new Vector3(rightBoundary + wrapThreshold , transform.position.y, transform.position.z);
        }
        else if (transform.position.x > rightBoundary + wrapThreshold)
        {
            transform.position = new Vector3(leftBoundary - wrapThreshold , transform.position.y, transform.position.z);
        }
    }

    private void MoveDiagonallyDownward()
    {
        // Move the lander diagonally downward
        float horizontalMove = moveRight ? horizontalSpeed : -horizontalSpeed;
        Vector3 moveDirection = new Vector3(horizontalMove, -descentSpeed, 0);
        transform.Translate(moveDirection * Time.deltaTime);

        // Check if the lander has reached or exceeded the low altitude
        if (transform.position.y <= lowAltitude)
        {
            currentState = MovementState.MovingHorizontally; // Change state
        }
    }

    private void MoveHorizontally()
    {
        moveRight = Random.value > 0.5f;
        // Continue moving in the initial horizontal direction
        float horizontalMove = moveRight ? horizontalSpeed : -horizontalSpeed;
        transform.Translate(Vector3.right * horizontalMove * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
