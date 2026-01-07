using UnityEngine;
using TMPro;

public class EndSceneUI : MonoBehaviour
{
    public TMP_Text thymeAmountText;
    public TMP_Text customerAmountText;
    public TMP_Text timeText;

    void Start()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        thymeAmountText.text =
            "You collected " + gm.thymeCollectedToday + " thymes";

        customerAmountText.text =
            "You served " + gm.customersServed + " customers";

        float timeSpent = gm.dayLength - gm.GetTimeRemaining();
        timeText.text =
            "In the time span of " + Mathf.RoundToInt(timeSpent) + " seconds";
    }
}
