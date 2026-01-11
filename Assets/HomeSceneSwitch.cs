using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneSwitch : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void Home()
    {
        StartCoroutine(LoadWithTransition("MenuScene"));
    }

    IEnumerator LoadWithTransition(string sceneName)
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
