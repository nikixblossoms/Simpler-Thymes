using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToFarm()
    {
        GameManager.Instance.LoadThymePlots();
        SceneManager.LoadScene("FarmScene");
    }

    public void GoToKitchen()
    {
        GameManager.Instance.SaveThymePlots();
        SceneManager.LoadScene("KitchenScene");
    }
}