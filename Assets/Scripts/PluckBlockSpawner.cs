using UnityEngine;

public class PluckBlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;

    [SerializeField] float minSpawnDistance = 30;
    [SerializeField] float maxSpawnDistance = 60;

    [SerializeField] float wallDistance = 4;
    [SerializeField] float levelScale = 4;

    [SerializeField] float maxSpawnTimer = 4;
    float spawnTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnTimer = maxSpawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0)
        {
            spawnTimer = maxSpawnTimer;
            spawnBlock();
        }
    }

    void spawnBlock()
    {
        GameObject tempBlock = Instantiate(blockPrefab);
        float angle = Mathf.Deg2Rad * Random.Range(0, 360);
        float blockX = Mathf.Cos(angle) * wallDistance * levelScale;
        float blockZ = Mathf.Sin(angle) * wallDistance * levelScale;
        tempBlock.transform.position = new Vector3(blockX, 20 + Random.Range(minSpawnDistance, maxSpawnDistance), blockZ);
    }
}
