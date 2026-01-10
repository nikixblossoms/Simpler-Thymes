using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [System.Serializable]
    public class ItemEntry
    {
        public string itemID;
        public GameObject prefab;
    }

    public List<ItemEntry> items = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetItemPrefab(string id)
    {
        foreach (var item in items)
        {
            if (item.itemID == id)
                return item.prefab;
        }

        return null;
    }
}
