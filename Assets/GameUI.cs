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

        thymeText.text = "Thyme: " + GameManager.Instance.thymeCount;

        customerText.text =
            "Customers: " +
            GameManager.Instance.customersServed + "/" +
            GameManager.Instance.customerGoal;

        timeText.text =
            "Time: " +
            Mathf.Ceil(GameManager.Instance.GetTimeRemaining());
    }
}
