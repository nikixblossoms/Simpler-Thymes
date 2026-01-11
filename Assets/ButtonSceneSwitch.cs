using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneSwitch : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void HowToPlay()
    {
        StartCoroutine(LoadWithTransition("HowToPlayScene"));
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
