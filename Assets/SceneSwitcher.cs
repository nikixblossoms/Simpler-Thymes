using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToFarm()
    {
        SceneManager.LoadScene("FarmScene");
    }

    public void GoToKitchen()
    {
        SceneManager.LoadScene("KitchenScene");
    }
}
