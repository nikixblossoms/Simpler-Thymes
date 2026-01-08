using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToSceneSwitch : MonoBehaviour
{
    public void Home()
    {
        SceneManager.LoadScene("MenuScene");
    }
}