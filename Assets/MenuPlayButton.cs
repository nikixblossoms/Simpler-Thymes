using System.Collections;
using UnityEngine;

public class MenuPlayButton : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void OnPlayPressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        StartCoroutine(PlayWithTransition());
    }

    IEnumerator PlayWithTransition()
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);

        GameManager.Instance.StartDay();
    }
}
