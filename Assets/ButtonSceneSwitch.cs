using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneSwitch : MonoBehaviour
{
    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlayScene");
    }
}