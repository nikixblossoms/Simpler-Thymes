using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneSwitch : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("KitchenScene");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlayScene");
    }
}