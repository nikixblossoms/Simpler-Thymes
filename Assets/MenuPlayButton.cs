using UnityEngine;

public class MenuPlayButton : MonoBehaviour
{
    public void OnPlayPressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        GameManager.Instance.StartDay();
    }
}
