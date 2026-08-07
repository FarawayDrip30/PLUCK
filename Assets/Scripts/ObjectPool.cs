using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] string poolName;
    public static Dictionary<string, ObjectPool> pools;

    [SerializeField] GameObject objectPrefab;

    [SerializeField] int initialAmount = 50;
    [SerializeField] int addendumAmount = 10;

    List<GameObject> objects;
    List<bool> objectsInUse;

    static Vector3 poolPos = new Vector3(-1000,-1000,-1000);

    void Awake()
    {
        if(pools == null)
        {
            pools = new Dictionary<string, ObjectPool>();
        }

        pools.Add(poolName, this);

        objects = new List<GameObject>();
        objectsInUse = new List<bool>();

        generateObjects(initialAmount);
    }

    public GameObject getObject()
    {
        for(int i = 0; i < objects.Count; i++)
        {
            if (!objectsInUse[i])
            {
                objectsInUse[i] = true;
                objects[i].SetActive(true);
                return objects[i];
            }
        }

        int nextFreeId = objects.Count;
        generateObjects(addendumAmount);
        objectsInUse[nextFreeId] = true;
        objects[nextFreeId].SetActive(true);
        return objects[nextFreeId];
    }

    public void generateObjects(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(objectPrefab);

            obj.transform.position = poolPos;
            obj.SetActive(false);

            objects.Add(obj);
            objectsInUse.Add(false);
        }
    }

    public void returnToPool(GameObject obj)
    {
        for(int i = 0; i < objects.Count; i++)
        {
            if(obj.GetInstanceID() == objects[i].GetInstanceID())
            {
                objects[i].transform.position = poolPos;
                objects[i].SetActive(false);
                objectsInUse[i] = false;
                return;
            }
        }
    }
}
