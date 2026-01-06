using UnityEngine;
using System.Collections;

public enum PlantState
{
    Empty,
    Growing,
    NeedsWater,
    Ready
}
public class ThymePlot : MonoBehaviour
{
    public PlantState state = PlantState.Empty;

    public Sprite emptySprite;
    public Sprite growingSprite;
    public Sprite needsWaterSprite;
    public Sprite readySprite;

    private SpriteRenderer sr;

    [Tooltip("Seconds the plant spends in the Growing state before needing water")]
    public float growTime = 2f;
    [Tooltip("Seconds after watering until the plant is Ready")]
    public float waterTime = 2f;

    // Tracks the currently running coroutine so we can stop or avoid duplicates
    private Coroutine growthCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("ThymePlot: Missing SpriteRenderer on " + gameObject.name, this);
        }
        if (emptySprite == null || growingSprite == null || needsWaterSprite == null || readySprite == null)
        {
            Debug.LogWarning("ThymePlot: One or more sprites are not assigned on " + gameObject.name, this);
        }

        Debug.Log("ThymePlot Awake: state=" + state + " on " + gameObject.name, this);
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
            case PlantState.Ready:
                sr.sprite = readySprite;
                break;
        }
    }

    public void Plant()
    {
        Debug.Log("ThymePlot.Plant() called on " + gameObject.name + " state=" + state, this);
        if (state != PlantState.Empty) return;

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
        Debug.Log("GrowRoutine started on " + gameObject.name + " (growTime=" + growTime + ")", this);

        float t = 0f;
        while (t < growTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Debug.Log("GrowRoutine completed on " + gameObject.name + ". Now NeedsWater.", this);
        state = PlantState.NeedsWater;
        UpdateVisual();

        growthCoroutine = null;
    }

    public void Water()
    {
        Debug.Log("ThymePlot.Water() called on " + gameObject.name + " state=" + state, this);
        if (state != PlantState.NeedsWater) return;

        if (growthCoroutine != null)
        {
            StopCoroutine(growthCoroutine);
            growthCoroutine = null;
        }
        growthCoroutine = StartCoroutine(SecondGrowRoutine());
    }

    IEnumerator SecondGrowRoutine()
    {
        Debug.Log("SecondGrowRoutine started on " + gameObject.name + " (waterTime=" + waterTime + ")", this);

        float t = 0f;
        while (t < waterTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Debug.Log("SecondGrowRoutine completed on " + gameObject.name + ". Now Ready.", this);
        state = PlantState.Ready;
        UpdateVisual();

        growthCoroutine = null;
    }

    public void Harvest()
    {
        Debug.Log("ThymePlot.Harvest() called on " + gameObject.name + " state=" + state, this);
        if (state != PlantState.Ready) return;

        int yield = Random.Range(1, 3);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.thymeCount += yield;
            Debug.Log("Harvested " + yield + " thyme. New total: " + GameManager.Instance.thymeCount, GameManager.Instance);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null - cannot add thyme", this);
        }

        state = PlantState.Empty;
        UpdateVisual();
    }
}
