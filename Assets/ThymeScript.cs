using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public enum PlantState
{
    Empty,
    Growing,
    NeedsWater,
    BeingWatered,
    Ready
}

public class ThymePlot : MonoBehaviour
{
    public PlantState state = PlantState.Empty;

    [Header("Sprites")]
    public Sprite emptySprite;
    public Sprite growingSprite;
    public Sprite needsWaterSprite;
    public Sprite beingWateredSprite;
    public Sprite readySprite;

    [Header("UI")]
    public Canvas progressCanvas;
    public Image progressFill;
    public TMP_Text progressText;

    [Header("Action Buttons")]
    public Button plantButton;
    public Button waterButton;
    public Button harvestButton;

    private SpriteRenderer sr;

    [Tooltip("Seconds the plant spends in the Growing state before needing water")]
    public float growTime = 2f;
    [Tooltip("Seconds after watering until the plant is Ready")]
    public float waterTime = 2f;

    private Coroutine growthCoroutine;

    void Awake()
    {
        if (progressCanvas != null)
            progressCanvas.gameObject.SetActive(false);

        sr = GetComponent<SpriteRenderer>();

        // Only initialize visuals if no saved state yet
        if (GameManager.Instance != null && GameManager.Instance.thymePlotsData.Count > 0)
            return;

        UpdateVisual();
        UpdateActionUI();
    }

    void UpdateVisual()
    {
        if (sr == null) return;

        switch (state)
        {
            case PlantState.Empty: sr.sprite = emptySprite; break;
            case PlantState.Growing: sr.sprite = growingSprite; break;
            case PlantState.NeedsWater: sr.sprite = needsWaterSprite; break;
            case PlantState.BeingWatered: sr.sprite = beingWateredSprite != null ? beingWateredSprite : needsWaterSprite; break;
            case PlantState.Ready: sr.sprite = readySprite; break;
        }
    }

    void UpdateActionUI()
    {
        if (plantButton != null)
            plantButton.gameObject.SetActive(state == PlantState.Empty);
        if (waterButton != null)
            waterButton.gameObject.SetActive(state == PlantState.NeedsWater);
        if (harvestButton != null)
            harvestButton.gameObject.SetActive(state == PlantState.Ready);
    }

    public void Plant()
    {
        if (state != PlantState.Empty) return;

        if (growthCoroutine != null) StopCoroutine(growthCoroutine);
        growthCoroutine = StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine(float startTime = 0f)
    {
        state = PlantState.Growing;
        UpdateVisual();
        UpdateActionUI();
        ShowProgressUI(true);

        float t = startTime;
        while (t < growTime)
        {
            t += Time.deltaTime;
            UpdateProgress(t, growTime);
            yield return null;
        }

        ShowProgressUI(false);
        state = PlantState.NeedsWater;
        UpdateVisual();
        UpdateActionUI();

        growthCoroutine = null;
    }

    public void Water()
    {
        if (state != PlantState.NeedsWater) return;

        if (growthCoroutine != null) StopCoroutine(growthCoroutine);
        growthCoroutine = StartCoroutine(WateringRoutine());
    }

    IEnumerator WateringRoutine(float startTime = 0f)
    {
        state = PlantState.BeingWatered;
        UpdateVisual();
        UpdateActionUI();
        ShowProgressUI(true);

        float t = startTime;
        while (t < waterTime)
        {
            t += Time.deltaTime;
            UpdateProgress(t, waterTime);
            yield return null;
        }

        ShowProgressUI(false);
        state = PlantState.Ready;
        UpdateVisual();
        UpdateActionUI();

        growthCoroutine = null;
    }

    public void Harvest()
    {
        if (state != PlantState.Ready) return;

        int yield = Random.Range(1, 3);
        if (GameManager.Instance != null)
            GameManager.Instance.CollectThyme(yield);

        state = PlantState.Empty;
        UpdateVisual();
        UpdateActionUI();
        ShowProgressUI(false);
    }

    void ShowProgressUI(bool show)
    {
        if (progressCanvas != null)
            progressCanvas.gameObject.SetActive(show);
    }

    void UpdateProgress(float current, float max)
    {
        if (progressFill != null)
            progressFill.fillAmount = current / max;
        if (progressText != null)
            progressText.text = Mathf.Ceil(max - current).ToString();
    }

    // -----------------------------
    // SAVE & LOAD
    // -----------------------------
    // Return the current plot data
public ThymePlotData GetData()
{
    float progress = 0f;
    if (state == PlantState.Growing || state == PlantState.BeingWatered)
    {
        if (progressFill != null)
            progress = progressFill.fillAmount; // store progress 0-1
    }
    return new ThymePlotData(state, progress);
}

// Load the plot data
    public void LoadData(ThymePlotData data)
    {
        state = data.state;
        UpdateVisual();
        UpdateActionUI();
        
        if (state == PlantState.Growing || state == PlantState.BeingWatered)
        {
            float totalTime = (state == PlantState.Growing) ? growTime : waterTime;
            float elapsedTime = data.progress * totalTime;
            if (growthCoroutine != null) StopCoroutine(growthCoroutine);

            growthCoroutine = StartCoroutine(ResumeCoroutine(elapsedTime));
        }
    }

    // Resume growing or watering from saved progress
    private IEnumerator ResumeCoroutine(float elapsed)
    {
        float totalTime = (state == PlantState.Growing) ? growTime : waterTime;
        state = (state == PlantState.Growing) ? PlantState.Growing : PlantState.BeingWatered;
        UpdateVisual();
        UpdateActionUI();
        ShowProgressUI(true);

        float t = elapsed;
        while (t < totalTime)
        {
            t += Time.deltaTime;
            UpdateProgress(t, totalTime);
            yield return null;
        }

        ShowProgressUI(false);
        state = (state == PlantState.Growing) ? PlantState.NeedsWater : PlantState.Ready;
        UpdateVisual();
        UpdateActionUI();

        growthCoroutine = null;
    }

}
