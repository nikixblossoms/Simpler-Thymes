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

    public Sprite emptySprite;
    public Sprite growingSprite;
    public Sprite needsWaterSprite;

    public Sprite beingWateredSprite;

    public Sprite readySprite;

    public Canvas progressCanvas;
    public Image progressFill;
    public TMP_Text progressText;

    private SpriteRenderer sr;

    [Tooltip("Seconds the plant spends in the Growing state before needing water")]
    public float growTime = 2f;
    [Tooltip("Seconds after watering until the plant is Ready")]
    public float waterTime = 2f;

    // Tracks the currently running coroutine so we can stop or avoid duplicates
    private Coroutine growthCoroutine;

    void Awake()
    {

        if (progressCanvas != null)
        {
            progressCanvas.gameObject.SetActive(false);
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("ThymePlot: Missing SpriteRenderer on " + gameObject.name, this);
        }
        if (emptySprite == null || growingSprite == null || needsWaterSprite == null || readySprite == null)
        {
            Debug.LogWarning("ThymePlot: One or more sprites are not assigned on " + gameObject.name, this);
        }
        UpdateVisual();
    }

    void UpdateVisual()
    {
    if (sr == null) return;

    switch (state)
        {
        case PlantState.Empty:
            sr.sprite = emptySprite;
            break;
        case PlantState.Growing:
            sr.sprite = growingSprite;
            break;
        case PlantState.NeedsWater:
            sr.sprite = needsWaterSprite;
            break;
        case PlantState.BeingWatered:
            sr.sprite = beingWateredSprite != null ? beingWateredSprite : needsWaterSprite;
            break;
        case PlantState.Ready:
            sr.sprite = readySprite;
            break;
        }   
    }


    public void Plant()
    {

        if (state != PlantState.Empty) return;
        ShowProgressUI(false);

        // Ensure only one growth coroutine runs at a time
        if (growthCoroutine != null)
        {
            StopCoroutine(growthCoroutine);
            growthCoroutine = null;
        }
        growthCoroutine = StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        state = PlantState.Growing;
        UpdateVisual();
        ShowProgressUI(true);

        float t = 0f;
        while (t < growTime)
        {
            t += Time.deltaTime;
            UpdateProgress(t, growTime);
            yield return null;
        }

        ShowProgressUI(false);
        state = PlantState.NeedsWater;
        UpdateVisual();

        growthCoroutine = null;
    }


    public void Water()
    {
    if (state != PlantState.NeedsWater) return;

    if (growthCoroutine != null)
    {
        StopCoroutine(growthCoroutine);
        growthCoroutine = null;
    }

    growthCoroutine = StartCoroutine(WateringRoutine());
    }   

    IEnumerator WateringRoutine()
    {
        state = PlantState.BeingWatered;
        UpdateVisual();
        ShowProgressUI(true);

        float t = 0f;
        while (t < waterTime)
        {
            t += Time.deltaTime;
            UpdateProgress(t, waterTime);
            yield return null;
        }

        ShowProgressUI(false);
        state = PlantState.Ready;
        UpdateVisual();

        growthCoroutine = null;
    }




    IEnumerator SecondGrowRoutine()
    {
        float t = 0f;
        while (t < waterTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        state = PlantState.Ready;
        UpdateVisual();

        growthCoroutine = null;
    }


    public void Harvest()
    {
        if (state != PlantState.Ready) return;

        int yield = Random.Range(1, 3);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.thymeCount += yield;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null - cannot add thyme", this);
        }

        ShowProgressUI(false);
        state = PlantState.Empty;
        UpdateVisual();
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

    
}
