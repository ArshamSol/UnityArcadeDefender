using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour
{
    public static ScrollingBackground Instance { get; private set; }
    public float scrollSpeed = 2f; // Scrolling speed
    public Transform[] backgroundParts;
    public SpriteRenderer spriteRenderer;
    public Camera mainCamera; // Main camera reference

    private float spriteWidth; // Width of each sprite
    private bool scrollingToRight = false; // Initially scrolling to the left
    private bool isWrapping = false;

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
            if (scrollingToRight && part.position.x > mainCamera.transform.position.x + spriteWidth)
            {
                Vector3 newPosition = part.position;
                newPosition.x -= spriteWidth * backgroundParts.Length;
                part.position = newPosition;
            }
            // Wrap around to the left
            else if (!scrollingToRight && part.position.x < mainCamera.transform.position.x - spriteWidth)
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
        isWrapping = true;
        StartCoroutine(SmoothWrapCoroutine(faceRight, duration, () => isWrapping = false));
    }

    // Smoothly wrap the background to position the ship on the other side with camera movement
    private IEnumerator SmoothWrapCoroutine(bool shipFaceRight, float duration, System.Action onComplete)
    {
        Vector3 cameraStartPosition = mainCamera.transform.position;
        Vector3 cameraTargetPosition = cameraStartPosition;
        cameraTargetPosition.x += shipFaceRight ? 16f : -16f;

        Vector3[] backgroundStartPositions = new Vector3[backgroundParts.Length];
        Vector3[] backgroundTargetPositions = new Vector3[backgroundParts.Length];

        for (int i = 0; i < backgroundParts.Length; i++)
        {
            backgroundStartPositions[i] = backgroundParts[i].position;
            backgroundTargetPositions[i] = backgroundParts[i].position + Vector3.right * (shipFaceRight ? -spriteWidth : spriteWidth);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            mainCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, t);
            
            for (int i = 0; i < backgroundParts.Length; i++)
            {
                backgroundParts[i].position = Vector3.Lerp(backgroundStartPositions[i], backgroundTargetPositions[i], t);
                Transform part = backgroundParts[i];

                if (!scrollingToRight && part.position.x > mainCamera.transform.position.x + spriteWidth)
                {
                    Vector3 newPosition = part.position;
                    newPosition.x -= spriteWidth * backgroundParts.Length;
                    part.position = newPosition;
                }
                else if (scrollingToRight && part.position.x < mainCamera.transform.position.x - spriteWidth)
                {
                    Vector3 newPosition = part.position;
                    newPosition.x += spriteWidth * backgroundParts.Length;
                    part.position = newPosition;
                }
            }
            yield return null;
        }

        scrollingToRight = !shipFaceRight;
        onComplete?.Invoke();
    }
}
