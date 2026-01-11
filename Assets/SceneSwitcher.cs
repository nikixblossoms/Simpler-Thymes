using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 0.5f;

    public void GoToFarm()
    {
        GameManager.Instance.LoadThymePlots();
        StartCoroutine(LoadWithDelay("FarmScene"));
    }

    public void GoToKitchen()
    {
        GameManager.Instance.SaveThymePlots();
        StartCoroutine(LoadWithDelay("KitchenScene"));
    }

    IEnumerator LoadWithDelay(string sceneName)
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
