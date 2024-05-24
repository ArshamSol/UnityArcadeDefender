using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject landerPrefab;
    public GameObject humanoidPrefab;
    public Transform backgroundPart;
    public Transform enemiesParent;

    private float spriteWidth; // Width of each background sprite
    private float spriteHeight;
    private float verticalMargin = 6f; // margin of vertical boundries for enemies position
    private int repeatTimes = 3; // How many times to repeat the batch spawn
    private int instancesPerBatch = 5; // Number of instances per batch
    private int score = 0;

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

    void Start()
    {
        spriteWidth = backgroundPart.GetComponent<SpriteRenderer>().bounds.size.x;
        spriteHeight = backgroundPart.GetComponent<SpriteRenderer>().bounds.size.y;
        StartCoroutine(SpawnRepeatedly());
    }

    private IEnumerator SpawnRepeatedly()
    {
        for (int i = 0; i < repeatTimes; i++)
        {
            for (int j = 0; j < instancesPerBatch; j++)
            {              
                Vector3 position = new Vector3(Random.Range(-spriteWidth, spriteWidth), spriteHeight - verticalMargin, 0f);
                GameObject enemyInstance = Instantiate(landerPrefab, position, Quaternion.identity, enemiesParent);                             
            }
            
            yield return new WaitForSeconds(5);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }
}