using UnityEngine;

public class Humanoid : MonoBehaviour
{
    private bool falling = false;
    private Vector3 fallVelocity = Vector3.zero;
    private float fallSpeed = 5f;
    private Transform initialPosition;

    void Start()
    {
        initialPosition = transform;
    }

    void Update()
    {
        if (falling)
        {
            fallVelocity.y = -fallSpeed;
            transform.position += fallVelocity * Time.deltaTime;

            if (transform.position.y <= initialPosition.position.y)
            {
                transform.position = initialPosition.position;
                falling = false;
            }
        }
    }

    public void StartFalling()
    {
        falling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(250); // Bonus for rescue
            falling = false;
            Destroy(gameObject);
        }
    }
}
