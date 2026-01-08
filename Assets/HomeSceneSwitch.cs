using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneSwitch : MonoBehaviour
{
    public void Home()
    {
        SceneManager.LoadScene("MenuScene");
    }
}