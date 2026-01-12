using UnityEngine;

public class MusicScript : MonoBehaviour
{
    private static MusicScript instance;

    void Awake()
    {
        // If a music instance already exists and it's not this one, destroy this
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this as the singleton instance
        instance = this;

        // Make this object persist between scenes
        DontDestroyOnLoad(gameObject);
    }
}
