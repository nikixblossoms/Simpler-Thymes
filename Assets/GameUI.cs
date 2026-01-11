using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI thymeText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI customerText;

    void Update()
    {
        if (GameManager.Instance == null) return;

        thymeText.text = "" + GameManager.Instance.thymeCount;

        customerText.text =
            "" +
            GameManager.Instance.customersServed + "/" +
            GameManager.Instance.customerGoal;

        timeText.text =
            "" +
            Mathf.Ceil(GameManager.Instance.GetTimeRemaining());
    }
}
