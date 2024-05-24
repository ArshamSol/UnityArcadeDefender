using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour
{
    public static ScrollingBackground Instance { get; private set; }
    public float scrollSpeed = 2f; // Scrolling speed
    public Transform[] backgroundParts;
    public Transform GameWorld;
    public SpriteRenderer spriteRenderer;
    public Camera mainCamera; // Main camera reference

    public float spriteWidth; // Width of each background sprite

    private bool scrollingToRight = false; // Initially scrolling to the left
    private bool isWrapping = false;
    private Coroutine currentWrapCoroutine = null;
    private Vector3 cameraInitialPosition;
    private enum Direction
    {
        Left = 0,
        Right = 1
    }
    private Direction initialShipDirection = Direction.Right;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        spriteWidth = spriteRenderer.bounds.size.x;
        cameraInitialPosition = mainCamera.transform.position;
    }

    void Update()
    {
        if (isWrapping)
            return;

        // Scroll the background horizontally
        float movement = scrollSpeed * Time.deltaTime * (scrollingToRight ? 1 : -1);
        transform.position += Vector3.right * movement;
        WrapBackground();
    }

    // Check if any background part needs to be wrapped
    private void WrapBackground()
    {
        for (int i = 0; i < backgroundParts.Length; i++)
        {
            Transform part = backgroundParts[i];

            // Wrap around to the right
            if (part.position.x > mainCamera.transform.position.x + spriteWidth)
            {
                Vector3 newPosition = part.position;
                newPosition.x -= spriteWidth * backgroundParts.Length;
                part.position = newPosition;
            }
            // Wrap around to the left
            else if (part.position.x < mainCamera.transform.position.x - spriteWidth) 
            {
                Vector3 newPosition = part.position;
                newPosition.x += spriteWidth * backgroundParts.Length;
                part.position = newPosition;
            }
        }
    }

    // Start smooth wrapping with camera transition when ship direction is changed
    public void StartSmoothWrap(bool faceRight, float duration)
    {
        // Stop the currently running wrap coroutine, if any
        if (currentWrapCoroutine != null)
        {
            StopCoroutine(currentWrapCoroutine);
        }

        isWrapping = true;
        currentWrapCoroutine = StartCoroutine(SmoothWrapCoroutine(faceRight, duration, () => {
            isWrapping = false;
            currentWrapCoroutine = null;
            cameraInitialPosition = mainCamera.transform.position;
            initialShipDirection = faceRight ? Direction.Right : Direction.Left;
        }));
    }

    // Smoothly wrap the background to position the ship on the other side with camera movement
    private IEnumerator SmoothWrapCoroutine(bool shipFaceRight, float duration, System.Action onComplete)
    {
        //float orthographicWidth = mainCamera.orthographicSize * mainCamera.aspect * 2f;
        float orthographicWidth = 16f; // TO FIX
        Vector3 cameraTargetPosition = Vector3.zero;
        Vector3 cameraStartPosition = mainCamera.transform.position;
        Direction currentShipDirection;

        currentShipDirection = shipFaceRight ? Direction.Right : Direction.Left;

        if (currentShipDirection == initialShipDirection) 
        { 
            cameraTargetPosition = cameraInitialPosition;
        }
        else 
        {
            cameraTargetPosition = cameraInitialPosition;
            cameraTargetPosition.x += shipFaceRight ? orthographicWidth : -orthographicWidth;
        }

        float distance = Vector3.Distance(cameraTargetPosition, cameraStartPosition);
        

        Vector3 gameWorldStartPosition = GameWorld.position;
        Vector3 gameWorldTargetPosition = GameWorld.position + Vector3.right * (shipFaceRight ? -distance : distance);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            mainCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, t);

            GameWorld.position = Vector3.Lerp(gameWorldStartPosition, gameWorldTargetPosition, t);
            WrapBackground();

            yield return null;
        }

        scrollingToRight = !shipFaceRight;
        onComplete?.Invoke();
    }
}