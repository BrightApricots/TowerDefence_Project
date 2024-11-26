using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private static ObjectPlacer instance;

    public static ObjectPlacer Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ObjectPlacer");
                instance = go.AddComponent<ObjectPlacer>();
                instance.placedGameObjects = new List<GameObject>();
            }
            return instance;
        }
    }

    [SerializeField]
    public List<GameObject> placedGameObjects = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (placedGameObjects == null)
        {
            placedGameObjects = new List<GameObject>();
        }
    }

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = Instantiate(prefab, position, rotation);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public void Clear()
    {
        //TODO : 싱글톤 이기 때문에 맵 변경시 초기화 필요함.
    }
}