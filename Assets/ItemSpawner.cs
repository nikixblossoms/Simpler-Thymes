using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject itemPrefab;   // Drag your prefab directly in Inspector
    public Transform spawnParent;   // Optional parent for default spawn
    public KitchenManager.RecipeStep stepForSlot; // Recipe step this item belongs to

    void Start()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("[ItemSpawner] No prefab assigned!");
            return;
        }

        // Check if prefab already exists in the scene to prevent duplicates
        DraggableItem prefabDraggable = itemPrefab.GetComponent<DraggableItem>();
        if (prefabDraggable == null)
        {
            Debug.LogError("[ItemSpawner] Prefab missing DraggableItem component!");
            return;
        }

        if (GameObject.FindObjectsOfType<DraggableItem>().Length > 0)
            return; // Already spawned at least once

        // Instantiate prefab
        GameObject spawnedItem = Instantiate(itemPrefab, spawnParent);

        // Optional: assign recipe step for this item
        SlotScript slot = spawnedItem.AddComponent<SlotScript>();
        slot.stepForThisSlot = stepForSlot;
    }
}
