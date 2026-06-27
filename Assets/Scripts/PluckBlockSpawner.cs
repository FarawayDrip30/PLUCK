using System.Collections.Generic;
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

    // How many blocks must exist before the chance that some get deleted occurs
    [SerializeField] int noDeleteCap = 25;
    [SerializeField] float deleteChance = 0.2f;
    [SerializeField] int deleteMin = 0;
    [SerializeField] int deleteMax = 10;

    List<GameObject> blocks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blocks = new List<GameObject>();
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
            
            if(blocks.Count >= noDeleteCap)
            {
                if(Random.Range(0.0f, 1.0f) < deleteChance)
                {
                    removeRandomBlocks(deleteMin, deleteMax);
                }
            }
        }
    }

    void spawnBlock()
    {
        GameObject tempBlock = Instantiate(blockPrefab);
        float angle = Mathf.Deg2Rad * Random.Range(0, 360);
        float blockX = Mathf.Cos(angle) * wallDistance * levelScale;
        float blockZ = Mathf.Sin(angle) * wallDistance * levelScale;
        tempBlock.transform.position = new Vector3(blockX, 20 + Random.Range(minSpawnDistance, maxSpawnDistance), blockZ);
        blocks.Add(tempBlock);
    }

    void removeRandomBlocks(int minBlocks, int maxBlocks)
    {
        int toDelete = Random.Range(minBlocks, maxBlocks);
        for (int i = 0; i < toDelete; i++)
        {
            if (blocks.Count > 0)
            {
                int removeId = Random.Range(0, blocks.Count);
                Destroy(blocks[removeId]);
                blocks.RemoveAt(removeId);
                Debug.Log("REMOVED");
            }
        }
    }
}
